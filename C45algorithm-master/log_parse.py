import json, sys


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