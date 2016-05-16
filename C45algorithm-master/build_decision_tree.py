import json, sys, math

# C4.5 implementation by geerk
import mine

# Parses .log files
from log_parse import get_next_block


'''
Class that stores all moves made by the player and the state that led to the move
'''
class StateDependentMoves(object):
	def __init__(self, file):
		self.src = open(file)
		self.curr_state = None
		self.table = []
		self.columns = ["result", "Player1 Current Life Points", "Player1 Favor", "Player1 Rally", "Player1 Balance", "Player1 Last Hit", "Player1 Last Attack by Player", "Player1 Landed Last Attack", "Player1 Last Evade", "Player1 Successful Evade", "Player1 Last Attack by Opponent", "Player1 Opponent Landed Last Attack", "Player1 [Surprise] Number of Attacks", "Player1 [Surprise] Number of Evades", "Player1 Distance to Opponent", "Player1 Winner", "Player2 Current Life Points", "Player2 Favor", "Player2 Rally", "Player2 Balance", "Player2 Last Hit", "Player2 Last Attack by Player", "Player2 Landed Last Attack", "Player2 Last Evade", "Player2 Successful Evade", "Player2 Last Attack by Opponent", "Player2 Opponent Landed Last Attack", "Player2 [Surprise] Number of Attacks", "Player2 [Surprise] Number of Evades", "Player1 Distance to Opponent", "Player2 Winner", "phase"]
		self.timing = {"Foward" : {"interval" : -1, "std_dev" : -1, "data_points" : []}, "Back" : {"interval" : -1, "std_dev" : -1, "data_points" : []}, "Down" : {"interval" : -1, "std_dev" : -1, "data_points" : []}, "Up" : {"interval" : -1, "std_dev" : -1, "data_points" : []}, "Button1" : {"interval" : -1, "std_dev" : -1, "data_points" : []}, "Button2" : {"interval" : -1, "std_dev" : -1, "data_points" : []}, "Button3" : {"interval" : -1, "std_dev" : -1, "data_points" : []}, "Button4" : {"interval" : -1, "std_dev" : -1, "data_points" : []}}
	
	'''
	Builds a data table
	'''
	def build_table(self):
		count = 0
		prev_game_time = -1
		prev_input = ""
		for block in get_next_block(self.src):
			if block[3] != "":
				# Update BlackBoard
				self.update_blackboard(block[3])
			else:
				# Compute timing information
				game_time = float(block[1])
				if prev_game_time > -1 and len(prev_input) > 0:
					self.timing[prev_input]["interval"] = math.fabs(prev_game_time - game_time) if self.timing[prev_input]["interval"] == -1 else self.timing[prev_input]["interval"] + math.fabs(prev_game_time - game_time)
					self.timing[prev_input]["data_points"].append(math.fabs(prev_game_time - game_time))
				
				prev_input = block[2]
				
				# Add a row to the data table; currently contains result
				row = [block[2]]
				# Player 1 data
				row.append(self.hp_level(float(self.curr_state["Player1"]["Current Life Points"])))
				row.append(self.favor(self.curr_state["Player1"]["Favor"]))
				row.append(self.rally(self.curr_state["Player1"]["Rally"]))
				row.append(self.balance(self.curr_state["Player1"]["Balance"]))
				row.append(self.last_hit_damage(self.curr_state["Player1"]["Last Hit"]))
				row.append(self.curr_state["Player1"]["Last Attack by Player"] if self.curr_state["Player1"]["Last Attack by Player"] != "" else "None")
				row.append(self.curr_state["Player1"]["Landed Last Attack"] if self.curr_state["Player1"]["Landed Last Attack"] != "" else "None")
				row.append(self.curr_state["Player1"]["Last Evade"] if self.curr_state["Player1"]["Last Evade"] != "" else "None")
				row.append(self.curr_state["Player1"]["Successful Evade"] if self.curr_state["Player1"]["Successful Evade"] != "" else "None")
				row.append(self.curr_state["Player1"]["Last Attack by Opponent"] if self.curr_state["Player1"]["Last Attack by Opponent"] != "" else "None")
				row.append(self.curr_state["Player1"]["Opponent Landed Last Attack"] if self.curr_state["Player1"]["Opponent Landed Last Attack"] != "" else "None")
				atk_eva = self.attack_evade(self.curr_state["Player1"]["[Surprise] Number of Attacks"], self.curr_state["Player1"]["[Surprise] Number of Evades"])
				row.append(atk_eva[0])
				row.append(atk_eva[1])
				row.append(self.is_close(self.curr_state["Player1"]["Distance to Opponent"]))
				row.append(self.curr_state["Player1"]["Winner"])
				# Player 2 data
				row.append(self.hp_level(float(self.curr_state["Player2"]["Current Life Points"])))
				row.append(self.favor(self.curr_state["Player2"]["Favor"]))
				row.append(self.rally(self.curr_state["Player2"]["Rally"]))
				row.append(self.balance(self.curr_state["Player2"]["Balance"]))
				row.append(self.last_hit_damage(self.curr_state["Player2"]["Last Hit"]))
				row.append(self.curr_state["Player2"]["Last Attack by Player"] if self.curr_state["Player2"]["Last Attack by Player"] != "" else "None")
				row.append(self.curr_state["Player2"]["Landed Last Attack"] if self.curr_state["Player2"]["Landed Last Attack"] != "" else "None")
				row.append(self.curr_state["Player2"]["Last Evade"] if self.curr_state["Player2"]["Last Evade"] != "" else "None")
				row.append(self.curr_state["Player2"]["Successful Evade"] if self.curr_state["Player2"]["Successful Evade"] != "" else "None")
				row.append(self.curr_state["Player2"]["Last Attack by Opponent"] if self.curr_state["Player2"]["Last Attack by Opponent"] != "" else "None")
				row.append(self.curr_state["Player2"]["Opponent Landed Last Attack"] if self.curr_state["Player2"]["Opponent Landed Last Attack"] != "" else "None")
				atk_eva = self.attack_evade(self.curr_state["Player2"]["[Surprise] Number of Attacks"], self.curr_state["Player2"]["[Surprise] Number of Evades"])
				row.append(atk_eva[0])
				row.append(atk_eva[1])
				row.append(self.is_close(self.curr_state["Player2"]["Distance to Opponent"]))
				row.append(self.curr_state["Player2"]["Winner"])
				
				# The last entry is a "phase" (early, mid, late) based on game time
				if game_time < 99 * 1/3.0:
					row.append("late")
				elif game_time < 99 * 2/3.0:
					row.append("mid")
				else:
					row.append("early")
				
				# Add to data table
				self.table.append(row)
				
				# Update game time
				prev_game_time = game_time
			
			count += 1
		
		# Calculate average and standard deviation of move information table
		for move in self.timing:
			self.timing[move]["interval"] = self.timing[move]["interval"] / len(self.timing[move]["data_points"]) if len(self.timing[move]["data_points"]) > 0 else -1
			self.timing[move]["std_dev"] = math.sqrt(sum([(self.timing[move]["interval"] - x_i) ** 2 for x_i in self.timing[move]["data_points"]])/(len(self.timing[move]["data_points"]))) if len(self.timing[move]["data_points"]) > 0 else -1
			self.timing[move].pop("data_points");
		
		# Convert data table to correct format and output it
		return self.to_dict()
	
	'''
	Reads a blackboard string... it should be in JSON format
	Uses the new string to update the blackboard state
	'''
	def update_blackboard(self, str_bb):
		self.curr_state = json.loads(str_bb)
	
	'''
	Convert the table to a dict format that c4.5 likes
	'''
	def to_dict(self):
		output = {}
		for i in range(0, len(self.columns)):
			output[self.columns[i]] = []
			
			# Get data for that column from the data table and put it in the dictionary
			for row in self.table:
				output[self.columns[i]].append(row[i])
		
		return output
	
	'''
	Miscellaneous calculations
	'''
	def hp_level(self, hp):
		if hp > 7500:
			return "> 7500"
		elif hp > 5000:
			return "> 5000"
		elif hp > 2500:
			return "> 2500"
		else:
			return "<= 2500"
	
	def last_hit_damage(self, last_hit):
		last_hit_damage = float(last_hit)
		if last_hit_damage == 0:
			return "0"
		elif last_hit_damage <= 50:
			return "<= 50"
		else:
			return "> 50"
	
	def favor(self, favor):
		favor_level = float(favor)
		if favor_level == 0:
			return "0"
		elif favor_level <= 25:
			return "<= 25"
		elif favor_level <= 50:
			return "<= 50"
		elif favor_level <= 75:
			return "<= 75"
		else:
			return "<= 100"
	
	def rally(self, rally):
		rally_level = float(rally)
		if rally_level == 0:
			return "0"
		elif rally_level <= 25:
			return "<= 25"
		elif rally_level <= 50:
			return "<= 50"
		elif rally_level <= 75:
			return "<= 75"
		else:
			return "<= 100"
	
	def balance(self, balance):
		balance_level = float(balance)
		if balance == 33:
			return "33"
		elif balance < 33:
			return "< 33"
		else:
			return "> 33"
	
	def attack_evade(self, attack_count, evade_count):
		total_moves = float(attack_count) + float(evade_count)
		if total_moves == 0:
			return ["about the same", "about the same"]
		elif abs((float(attack_count) - float(evade_count))/total_moves) < .05:
			return ["about the same", "about the same"]
		elif float(attack_count) > float(evade_count):
			return ["more", "less"]
		else:
			return ["less", "more"]
	
	def is_close(self, dist_to_opponent):
		return "Close" if float(dist_to_opponent) < 3.5 else "Far"


if __name__ == "__main__":
	logfile_name = sys.argv[1] if len(sys.argv) == 2 else "shahan.log"
	sdm = StateDependentMoves(logfile_name)
	
	# Build the data table
	data_table = sdm.build_table()
	
	# Build decision tree rules
	mine.validate_table(data_table)
	with open(logfile_name[:len(logfile_name) - len(".log")] + ".json", "w") as f:
		json.dump([json.loads(mine.tree_to_json(mine.mine_c45(data_table, "result"))), sdm.timing], f, indent=4, separators=(',', ': '))
		f.close()