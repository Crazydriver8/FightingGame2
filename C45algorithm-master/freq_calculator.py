import json, sys

# Parses .log files
from log_parse import get_next_block


'''
Stores distributions from a file
'''
class Distributions(object):
	def __init__(self, filename):
		self.src = open(filename)
		self.params = {"P1 Wins" : 0, "P2 Wins" : 0, "Foward" : 0, "Backward" : 0, "Up" : 0, "Down" : 0, "Button1" : 0, "Button2" : 0, "Button3" : 0, "Button4" : 0}
		self.buttons = 0
		self.games = 0
	
	def get_distribution_table(self):
		for block in get_next_block(self.src):
			if block[3] != "":
				# Extract a (possible) winner from the BlackBoard
				bb_state = json.loads(block[3])
				
				if bb_state["Player1"]["Winner"] == "true":
					self.params["P1 Wins"] += 1
					self.games += 1
				
				if bb_state["Player2"]["Winner"] == "true":
					self.params["P2 Wins"] += 1
					self.games += 1
			else:
				if block[2] in self.params:
					self.params[block[2]] += 1
					self.buttons += 1


if __name__ == "__main__":
	# Build the data table
	data_table = Distributions(sys.argv[1]) if sys.argv[1:] else None
	
	# Build distribution
	if data_table:
		data_table.get_distribution_table()
		
		# Output distribution as JSON
		with open(sys.argv[1][:len(sys.argv[1]) - len(".log")] + "_distr.json", "w") as f:
			json.dump(data_table.params, f, indent=4, separators=(',', ': '))
	
	else:
		# Did not receive a .log file to process
		print "Error : No data"