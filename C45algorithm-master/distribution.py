'''
For a list l:
	Get number of elements in list
	Get distribution of different elements
'''
def get_distribution(l):
    distributions = {}
    for i in l:
        if not i in distributions:
            distributions[i] = 1
        else:
            distributions[i] += 1
    
    return [len(l) * 1.0, distributions]