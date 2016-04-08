import json

# C4.5 implementation by geerk
import mine


'''
Generator that gets blocks from an opened file
A block is as follows: [player_name, timestamp, type, blackboard]
'''
def get_next_block(file):
	while True:
		next = file.readline()
		while next == "\n":
			next = file.readline()
		
		player_name = "".join(next.split("\n"))
		
		# EOF?
		if not player_name:
			break
		else:
			timestamp = "".join(file.readline().split("\n"))
			type = "".join(file.readline().split("\n"))
			blackboard = ""
			
			if type == "BlackBoard Update":
				line = file.readline()
				while line != "}\n":
					# Ensure valid JSON format
					if line[len(line) - 3:] == "},\n" and blackboard[-1] == ",":
						blackboard = blackboard[:-1]
					blackboard += "".join("".join(line.split("\n")).split("\t"))
					line = file.readline()
				
				# Add the last part
				if blackboard[-1] == ",":
					blackboard = blackboard[:-1]
				blackboard += "".join("".join(line.split("\n")).split("\t"))
			
			yield [player_name, timestamp, type, blackboard]


'''
Class that stores all moves made by the player and the state that led to the move
'''
class StateDependentMoves(object):
	def __init__(self, file):
		self.src = open(file)
		self.curr_state = None
		self.table = []
		self.columns = ["result", "Player1 Current Life Points", "Player1 Favor", "Player1 Rally", "Player1 Balance", "Player1 Last Hit", "Player1 Last Attack by Player", "Player1 Landed Last Attack", "Player1 Last Evade", "Player1 Successful Evade", "Player1 Last Attack by Opponent", "Player1 Opponent Landed Last Attack", "Player1 [Surprise] Number of Attacks", "Player1 [Surprise] Number of Evades", "Player1 Distance to Opponent", "Player1 Winner", "Player2 Current Life Points", "Player2 Favor", "Player2 Rally", "Player2 Balance", "Player2 Last Hit", "Player2 Last Attack by Player", "Player2 Landed Last Attack", "Player2 Last Evade", "Player2 Successful Evade", "Player2 Last Attack by Opponent", "Player2 Opponent Landed Last Attack", "Player2 [Surprise] Number of Attacks", "Player2 [Surprise] Number of Evades", "Player1 Distance to Opponent", "Player2 Winner", "phase"]
	
	'''
	Builds a data table
	'''
	def build_table(self):
		for block in get_next_block(self.src):
			if block[3] != "":
				# Update BlackBoard
				self.update_blackboard(block[3])
			else:
				# Add a row to the data table; currently contains result
				row = [block[2]]
				# Player 1 data
				row.append(self.hp_level(float(self.curr_state["Player1"]["Current Life Points"])))
				row.append(self.curr_state["Player1"]["Favor"])
				row.append(self.curr_state["Player1"]["Rally"])
				row.append(self.curr_state["Player1"]["Balance"])
				row.append(self.curr_state["Player1"]["Last Hit"])
				row.append(self.curr_state["Player1"]["Last Attack by Player"] if self.curr_state["Player1"]["Last Attack by Player"] != "" else "None")
				row.append(self.curr_state["Player1"]["Landed Last Attack"] if self.curr_state["Player1"]["Landed Last Attack"] != "" else "None")
				row.append(self.curr_state["Player1"]["Last Evade"] if self.curr_state["Player1"]["Last Evade"] != "" else "None")
				row.append(self.curr_state["Player1"]["Successful Evade"] if self.curr_state["Player1"]["Successful Evade"] != "" else "None")
				row.append(self.curr_state["Player1"]["Last Attack by Opponent"] if self.curr_state["Player1"]["Last Attack by Opponent"] != "" else "None")
				row.append(self.curr_state["Player1"]["Opponent Landed Last Attack"] if self.curr_state["Player1"]["Opponent Landed Last Attack"] != "" else "None")
				row.append(self.curr_state["Player1"]["[Surprise] Number of Attacks"])
				row.append(self.curr_state["Player1"]["[Surprise] Number of Evades"])
				row.append("Close" if float(self.curr_state["Player1"]["Distance to Opponent"]) < 3.5 else "Far")
				row.append(self.curr_state["Player1"]["Winner"])
				# Player 2 data
				row.append(self.hp_level(float(self.curr_state["Player2"]["Current Life Points"])))
				row.append(self.curr_state["Player2"]["Favor"])
				row.append(self.curr_state["Player2"]["Rally"])
				row.append(self.curr_state["Player2"]["Balance"])
				row.append(self.curr_state["Player2"]["Last Hit"])
				row.append(self.curr_state["Player2"]["Last Attack by Player"] if self.curr_state["Player2"]["Last Attack by Player"] != "" else "None")
				row.append(self.curr_state["Player2"]["Landed Last Attack"] if self.curr_state["Player2"]["Landed Last Attack"] != "" else "None")
				row.append(self.curr_state["Player2"]["Last Evade"] if self.curr_state["Player2"]["Last Evade"] != "" else "None")
				row.append(self.curr_state["Player2"]["Successful Evade"] if self.curr_state["Player2"]["Successful Evade"] != "" else "None")
				row.append(self.curr_state["Player2"]["Last Attack by Opponent"] if self.curr_state["Player2"]["Last Attack by Opponent"] != "" else "None")
				row.append(self.curr_state["Player2"]["Opponent Landed Last Attack"] if self.curr_state["Player2"]["Opponent Landed Last Attack"] != "" else "None")
				row.append(self.curr_state["Player2"]["[Surprise] Number of Attacks"])
				row.append(self.curr_state["Player2"]["[Surprise] Number of Evades"])
				row.append("Close" if float(self.curr_state["Player2"]["Distance to Opponent"]) < 3.5 else "Far")
				row.append(self.curr_state["Player2"]["Winner"])
				
				# The last entry is a "phase" (early, mid, late) based on game time
				game_time = float(block[1])
				if game_time < 99 * 1/3.0:
					row.append("late")
				elif game_time < 99 * 2/3.0:
					row.append("mid")
				else:
					row.append("early")
				
				# Add to data table
				self.table.append(row)
		
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


if __name__ == "__main__":
	# Build the data table
	data_table = StateDependentMoves("idgaff.log").build_table()
	#print json.dumps(data_table)
	
	# Build decision tree rules
	mine.validate_table(data_table)
	print mine.tree_to_rules(mine.mine_c45(data_table, "result"))
	
	#with open("table.json") as f:
	#	print mine.tree_to_rules(mine.mine_c45(json.loads(f.read()), "result"))
	#	f.close()