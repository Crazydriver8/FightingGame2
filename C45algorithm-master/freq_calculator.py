import json
import sys


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
	data_table = Distributions(sys.argv[1] if len(sys.argv) == 2 else "jojo.log")
	data_table.get_distribution_table()
	
	print data_table.buttons,
	print " buttons pressed"
	
	print data_table.games,
	print " games played"
	
	#for key in data_table.params:
	#	print key,
	#	print " : ",
	#	print data_table.params[key]
	
	print json.dumps(data_table.params)