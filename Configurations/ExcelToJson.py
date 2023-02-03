import json
import os
import pandas
import click

def work(exportPath):
    folder_name = exportPath
    os.mkdir(folder_name)
    dir_list = []
    for root, dirs, files in os.walk("."):
        if "Tools" in root:
    	    continue
        for dir in dirs:
            if dir == "Tools":
                continue
            dir_list.append(dir)

    for dir in dir_list:
        #针对不同的文件夹构建配置文件
        os.mkdir(folder_name + "/" + dir)
        file_list = []
        for root, dirs, files in os.walk("./" + dir):
            for file_name in files:
                if not file_name.endswith(".xlsx"):
                    continue
                if file_name.startswith("~"):
                    continue
                file_list.append(file_name)
        file_obj_list = []
        for file_name in file_list:
            file = pandas.ExcelFile(dir + "/" + file_name)
            file_obj_list.append(file)
            constructJson(file, folder_name + "/" + dir)
        print("==================== construct meta file ====================")
        list = []
        for file in file_obj_list:
            for sheet_name in file.sheet_names:
                list.append(sheet_name + ".json")
        metaFile = {'file_list': list}
        with open(folder_name + "/" + dir + "/meta.json", 'w') as fObj:
            fObj.write(json.dumps(metaFile, indent=4, sort_keys=True))
        print("====================         done " + dir + "        ==================== ")
                

    
def constructJson(file, folder_name):
    for sheet_name in file.sheet_names:
        sheet = pandas.read_excel(file, sheet_name)
        sheet = sheet.loc[:, ~sheet.columns.str.contains('^Unnamed')]
        sheet = sheet.dropna(how='any',axis=0)
        json_file_name = folder_name + "/" + sheet_name + ".json"
        print("write " + json_file_name)
        if sheet_name == "building_resource_config" or sheet_name == "lab_item_config" or sheet_name == "building_production_config" or sheet_name == "building_cap_config":
        
            res_list = []
            production_list = []
            repair_list = []
            cap_list = []
            
            res_index_list = []
            cap_index_list = []
            production_index_list = []
            repair_index_list = []
            
            
            index = 0
            for col in sheet:
                if col.startswith("res_"):
                    res_list.append(col)
                    res_index_list.append(index)
                elif col.startswith("p_"):
                    production_list.append(col)
                    production_index_list.append(index)
                elif col.startswith("repair_"):
                    repair_list.append(col)
                    repair_index_list.append(index)
                elif col.startswith("cap_"):
                    cap_list.append(col)
                    cap_index_list.append(index)
                index = index + 1
            
            res_str_list = []
            pro_str_list = []
            repair_str_list = []
            cap_str_list = []
            sub_res_str_list = []
            
            for index, row in sheet.iterrows():
                production_str = "["
                resource_str = "["
                repair_str = "["
                cap_str = "["
                sub_res_str = "["
                index = 0
                for value in row:
                    if value == 0:
                        index = index + 1
                        continue
                    if index in production_index_list:
                        production_str += "{\"key\":\"" + production_list[production_index_list.index(index)][2:] + "\", \"count\":" + str(value) + "}, "
                    elif index in res_index_list:
                        resource_str += "{\"key\":\"" + res_list[res_index_list.index(index)][4:] + "\", \"count\":" + str(value) + "}, "
                    elif index in repair_index_list:
                        repair_str += "{\"key\":\"" + repair_list[repair_index_list.index(index)][7:] + "\", \"count\":" + str(value) + "}, "
                    elif index in cap_index_list:
                        cap_str += "{\"key\":\"" + cap_list[cap_index_list.index(index)][4:] + "\", \"count\":" + str(value) + "}, "
                    index = index + 1
                                
                if len(production_str) > 2:
                    res_str_list.append(production_str[:-2] + "]")
                else:
                    res_str_list.append("[]")
                    
                if len(resource_str) > 2:
                    pro_str_list.append(resource_str[:-2] + "]")
                else:
                    pro_str_list.append("[]")
                    
                if len(repair_str) > 2:
                    repair_str_list.append(repair_str[:-2] + "]")
                else:
                    repair_str_list.append("[]")
                    
                if len(cap_str) > 2:
                    cap_str_list.append(cap_str[:-2] + "]")
                else:
                    cap_str_list.append("[]")

            sheet.drop(sheet.columns[res_index_list + production_index_list + repair_index_list + cap_index_list], axis=1, inplace=True)
            
            if len(production_list) > 0:
                sheet['resource_production_list'] = pandas.DataFrame(res_str_list)
            if len(res_list) > 0:
                sheet['resource_modification_list'] = pandas.DataFrame(pro_str_list)
            if len(repair_list) > 0:
                sheet['repair_resource_list'] = pandas.DataFrame(repair_str_list)
            if len(cap_list) > 0:
                sheet['capacity_modification_list'] = pandas.DataFrame(cap_str_list)
            
        json_str = sheet.to_json(orient='records')
        json_str = json_str.replace(".0,", ",")
        json_str = json_str.replace(".0}", "}")
        json_str = json_str.replace("\\\"", "\"")
        json_str = json_str.replace("\"[", "[")
        json_str = json_str.replace("]\"", "]")
        json_str = "{\"key\":\"0\",\"content\":" + json_str + "}";
        with open(json_file_name, 'w') as fObj:
            parsed = json.loads(json_str)
            fObj.write(json.dumps(parsed, indent=4, sort_keys=True))

work("../Assets/StreamingAssets/Config")
