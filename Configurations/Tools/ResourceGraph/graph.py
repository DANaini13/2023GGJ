import json
import matplotlib.pyplot as plt
import pandas


res_name_list = ["pop", "dec", "food", "ink", "wood", "stone", "aura"]
res_last_list = [-1, -1, -1, -1, -1, -1, -1]
res_summary_list = [0, 0, 0, 0, 0, 0, 0]
res_mines_list = [0, 0, 0, 0, 0, 0, 0]

path = 'data.json'
file = open(path, 'r', encoding='utf-8')
data = json.load(file)
format_data = []
format_data_1 = []
format_data_2 = []
for unit in data:
	data_unit = {}
	data_unit["time"] = unit["game_time"]
	for res_pair in unit["resource_count_list"]:
		data_unit[res_name_list[res_pair["resource_id"]]] = res_pair["count"]
		current = res_pair["count"]
		last = res_last_list[res_pair["resource_id"]]
		if last == -1:
			res_last_list[res_pair["resource_id"]] = current
		res_last_list[res_pair["resource_id"]] = res_pair["count"]
		if current < last:
			res_mines_list[res_pair["resource_id"]] += last - current
		elif current > last:
			res_summary_list[res_pair["resource_id"]] += current - last
		
	format_data.append(data_unit)

	data_unit_1 = {}
	data_unit_1["time"] = unit["game_time"]
	for res_pair in unit["resource_count_list"]:
		data_unit_1[res_name_list[res_pair["resource_id"]]] = res_summary_list[res_pair["resource_id"]]
	format_data_1.append(data_unit_1)

	data_unit_2 = {}
	data_unit_2["time"] = unit["game_time"]
	for res_pair in unit["resource_count_list"]:
		data_unit_2[res_name_list[res_pair["resource_id"]]] = res_mines_list[res_pair["resource_id"]]
	format_data_2.append(data_unit_2)

df = pandas.DataFrame(format_data)
df.to_excel("./resourceChange.xlsx")
df = pandas.DataFrame(format_data_1)
df.to_excel("./incomeHistory.xlsx")
df = pandas.DataFrame(format_data_2)
df.to_excel("./outcomeHistory.xlsx")