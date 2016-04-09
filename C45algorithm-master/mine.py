import json

from utils import get_subtables, formalize_rules, is_mono
from c45 import gain

from distribution import get_distribution

def mine_c45(table, result):
    """ An entry point for C45 algorithm.

        _table_ - a dict representing data table in the following format:
        {
            "<column name>': [<column values>],
            "<column name>': [<column values>],
            ...
        }

        _result_: a string representing a name of column indicating a result.
    """
    tree = []
    
    # Special case when there is a mixed strategy
    if len(table.keys()) == 1:
        key_distr = get_distribution(table[result])
        for k in key_distr[1].keys():
            tree.append(['probability %f' % (key_distr[1][k] / key_distr[0]),
                         '%s=%s' % (result, k)])
        
        return tree
    
    # All other cases
    col = max([(k, gain(table, k, result)) for k in table.keys() if k != result],
              key=lambda x: x[1])[0]
    for subt in get_subtables(table, col):
        v = subt[col][0]
        if is_mono(subt[result]):
            tree.append(['%s=%s' % (col, v),
                         '%s=%s' % (result, subt[result][0])])
        else:
            del subt[col]
            tree.append(['%s=%s' % (col, v)] + mine_c45(subt, result))
    
    return tree

def tree_to_rules(tree):
    return formalize_rules(__tree_to_rules(tree))


def __tree_to_rules(tree, rule=''):
    rules = []
    for node in tree:
        if isinstance(node, basestring):
            rule += node + ','
        else:
            rules += __tree_to_rules(node, rule)
    if rules:
        return rules
    return [rule]

def tree_to_json(tree):
    return json.dumps(__tree_to_dict(tree))

def __tree_to_dict(tree, parent = {}):
    curr_key = ""
    
    for node in tree:
        if isinstance(node, basestring):
            if "result" in node:
                parent[curr_key] = parent[curr_key] if curr_key in parent and parent[curr_key] else []
                parent[curr_key].append(node[7:])
            elif not parent or not node in parent:
                parent[node] = {}
                curr_key = node
            else:
                curr_key = node
        else:
            if curr_key != "":
                parent[curr_key] = __tree_to_dict(node, parent[curr_key])
            else:
                parent = __tree_to_dict(node, parent)
    
    return parent

def validate_table(table):
    assert isinstance(table, dict)
    for k, v in table.items():
        assert k
        assert isinstance(k, basestring)
        assert len(v) == len(table.values()[0])
        for i in v: assert i
