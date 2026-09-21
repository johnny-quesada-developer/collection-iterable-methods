# Benchmark experiment archive

Historical engineering notes from 2026-09-20. The current performance report is [here](README.md).

The redesign retains typed-array indexed fast paths while restoring the original generic indexed loops. Slice now uses LINQ partitions without cancellation and one bounded enumerator with cancellation. The parallel-filter scratch-buffer experiment improved cheap predicates but slowed CPU-heavy callbacks by about 8–9%; it was rejected. Default parallel filtering retains the original ConcurrentBag algorithm.

**Not every row is an optimization.** Generic indexed loops are essentially unchanged. MapIndexed is retained for the approved type generalization. Async completion/cancellation and configured parallel options include correctness fixes. ConcatAsync measures 0.92–0.99× across these runs; no performance gain is claimed. Some parallel paths also measure slower. These results remain visible rather than being counted as wins; small differences cannot automatically be dismissed as noise or attributed to a particular change. The streaming ToRecord case at 32 elements is 1.02× in the final run and 0.98× in the repeat, so no repeatable gain is established there.

Historical after.json/confirmation.json and intermediate experiment files are preserved but superseded by retained-final.json and retained-repeat.json.

