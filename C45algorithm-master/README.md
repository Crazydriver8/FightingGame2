This is a collection of Python scripts for data analysis and AI generation
It is based off of a fork of https://github.com/geerk/C45algorithm

After pulling a player's `player.log` file from the server, run
```
python build_decision_tree.py player.log
```
to construct a decision tree for that player. The RuleBasedAI2.cs file can use the generated `player.json` file as a possible decision tree AI. Note that, due to UFE limitations, the AI must be loaded into UFE when the game starts.

To perform data analysis, given a `player.log` file generated from having a human play the game and a corresponding `player_AI.log` file generated from having the player's AI face off against the UFE AI, use...
```
python freq_distribution player.log
python freq_distribution player_AI.log
```
to generate `player_distr.json` and `player_AI_distr.json`. These two files can be used with `goodness_of_fit.py` to generate a chi-squared analysis:
```
python goodness_of_fit.py player_distr.json player_AI_distr.json
```
Included are a very small sample of the data collected (in `.log` files), the generated decision trees and frequency distributions (in `.json` files), and the resulting chi-squared analyses (in `.chi` files). Including any more than what is given would use too much space, so many of the results had to be excluded from the GitHub commit.
