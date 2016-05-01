import sys, json

from scipy.stats import chisquare


# Converts a dictionary of frequencies to a list of percentages in order to do chi-squared testing
# Format is already known
def distr_to_percent(distr):
	# The first 2 are win rates
	rounds = distr["P1 Wins"] + distr["P2 Wins"]
	p1_win_rate = distr["P1 Wins"] * 1.0 / rounds
	p2_win_rate = distr["P2 Wins"] * 1.0 / rounds
	
	# The rest are button presses
	buttons_pressed = distr["Foward"] + distr["Backward"] + distr["Up"] + distr["Down"] + distr["Button1"] + distr["Button2"] + distr["Button3"] + distr["Button4"]
	foward_rate = distr["Foward"] * 1.0 / buttons_pressed
	backward_rate = distr["Backward"] * 1.0 / buttons_pressed
	up_rate = distr["Up"] * 1.0 / buttons_pressed
	down_rate = distr["Down"] * 1.0 / buttons_pressed
	button1_rate = distr["Button1"] * 1.0 / buttons_pressed
	button2_rate = distr["Button2"] * 1.0 / buttons_pressed
	button3_rate = distr["Button3"] * 1.0 / buttons_pressed
	button4_rate = distr["Button4"] * 1.0 / buttons_pressed
	
	return [p1_win_rate, p2_win_rate, foward_rate, backward_rate, up_rate, down_rate, button1_rate, button2_rate, button3_rate, button4_rate]


if __name__ == "__main__":
	player_distr = None
	if sys.argv[1]:
		with open(sys.argv[1]) as player_file:
			player_distr = json.loads(player_file.read())
			player_file.close()
	
	ai_distr = None
	if sys.argv[2]:
		with open(sys.argv[2]) as ai_file:
			ai_distr = json.loads(ai_file.read())
			ai_file.close()
	
	if player_distr and ai_distr:
		# Convert to percentages
		player_distr_percent = distr_to_percent(player_distr)
		ai_distr_percent = distr_to_percent(ai_distr)
		
		# Run the test
		print chisquare(ai_distr_percent, f_exp = player_distr_percent)
	else:
		print "Test failed; no data given"