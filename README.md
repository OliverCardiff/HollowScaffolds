# HollowScaffolds
Rewrites Genome Scaffolds with Ns via 2-MA coverage intersect

This adopts the double rolling mean fragmentation method for read-coverage-fragmenting scaffolds, and instead replaces characters between fragmentation points with Ns.

![Core Algorithm Summary](https://github.com/OliverCardiff/HollowScaffolds/blob/master/double_rmean.png)

Usage:

1. Edit Program.cs before compilation (LOWCOV, HIGHCOV, and RANGE variables)

These should be set to read-coverage density peaks found in multi-modal read-coverage assemblies.

2. Convert your BAM file to a wig file with something like https://github.com/MikeAxtell/bam2wig

3. ./Hollow_Scaffs <genome.fasta> <wigfile.wig> <output_prefix>

Output:

Two fasta files: <prefix>_highs.fasta & <prefix>_lows.fasta

These are the exclusively consistently high and low coverage segments from the assembly.
