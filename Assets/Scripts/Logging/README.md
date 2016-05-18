# Distribution

This class manages the bar graphs for frequency distributions

On successfully `Increment()`'ing the counter for a button, all bar graphs are redrawn. Sample usage:
```
Distribution d = ...;
if (d.Increment(<button>))
  d.DrawGraph();
```
