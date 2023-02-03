import click
import pandas

@click.command()
@click.option('--path', prompt='文件路径', help='包含building_config表的.xlsx文件')
def setPath(path):
    # clean the space in path
    path.replace(' ', '')
    click.echo(f"Path: {path}")
    file = pandas.ExcelFile(path)
    foundSheet = False
    for sheet_name in file.sheet_names:
        if sheet_name != "building_config":
            continue
        foundSheet = True
        break
    if not foundSheet:
        click.echo('该表中不存在building_config!')
        return
    sheet = pandas.read_excel(file, 'building_config')
    # construct group file by building id
    building_list = []
    id = 0
    for key in sheet['key']:
        building_list.append((id, key))
        id += 1
    buildingIdFile = []
    for buildingInfo in building_list:
        for buildingInfo1 in building_list:
            if buildingInfo[0] < buildingInfo1[0]:
                continue
            buildingIdFile.append({
            "from_building_id": buildingInfo[0],
            "to_building_id": buildingInfo1[0],
            "ratio": 0,
            "from_building_key": buildingInfo[1],
            "to_building_key": buildingInfo1[1]})
    df = pandas.DataFrame(buildingIdFile)
    df.to_excel("./buffByBuildingId.xlsx")
    # construct group file by building type
    buildingTypeFile = []
    buildingTypeList = []
    for type in sheet['building_type']:
        if type in buildingTypeList:
            continue
        buildingTypeList.append(type)
        
    for type in buildingTypeList:
        for type1 in buildingTypeList:
            if type < type1:
                continue
            buildingTypeFile.append({"from_type": type, "to_type": type1, "ratio": 0})
    df = pandas.DataFrame(buildingTypeFile)
    df.to_excel("./buffByBuildingType.xlsx")
    
if __name__ == '__main__':
    setPath()
