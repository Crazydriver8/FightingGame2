import sys, json

from scipy.stats import chisquare


# Read distribution from file
def read_distr(filename):
	if filename:
		with open(filename) as f:
			distr = json.loads(f.read())
			f.close()
			return distr
	else:
		return None

# Converts a dictionary of frequencies to a list of percentages in order to do chi-squared testing
# Format is already known
def distr_to_percent(distr):
	rounds = distr["P1 Wins"] + distr["P2 Wins"] # Get total rounds played
	buttons_pressed = distr["Foward"] + distr["Backward"] + distr["Up"] + distr["Down"] + distr["Button1"] + distr["Button2"] + distr["Button3"] + distr["Button4"] # Get total buttons pressed
	
	#distr["P1 Wins"] * 1.0 / rounds, distr["P2 Wins"] * 1.0 / rounds, 
	result = [distr["Foward"] * 1.0 / buttons_pressed, distr["Backward"] * 1.0 / buttons_pressed, distr["Up"] * 1.0 / buttons_pressed, distr["Down"] * 1.0 / buttons_pressed, distr["Button1"] * 1.0 / buttons_pressed, distr["Button2"] * 1.0 / buttons_pressed, distr["Button3"] * 1.0 / buttons_pressed, distr["Button4"] * 1.0 / buttons_pressed]
	
	for i in range(0, len(result)):
		result[i] = result[i] if result[i] > 0 else 0.00001
	
	return result


if __name__ == "__main__":
	player_distr = read_distr(sys.argv[1]) if sys.argv[1:] else None
	ai_distr = read_distr(sys.argv[2]) if sys.argv[2:] else None
	
	if player_distr and ai_distr:
		# Convert to percentages
		player_distr_percent = distr_to_percent(player_distr)
		ai_distr_percent = distr_to_percent(ai_distr)
		
		# Run the test; Power_divergenceResult(statistic=0.0, pvalue=1.0) when the two lists are identical
		print chisquare(ai_distr_percent, f_exp = player_distr_percent)
	else:
		print "Test failed; no data given"