## Benchmark - [source code](https://github.com/blackbone/other-ecs-benchmarks/pull/7/commits/157e2faecd08921fc8ac90ef224ad0b4615bd4b3)

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19045.5131/22H2/2022Update)
AMD Ryzen 7 4800H with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.404
[Host]     : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX2
Job-XWDGMM : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX2

Method=Run InvocationCount=1 RunStrategy=Throughput  
UnrollFactor=1

# Add1ComponentRandomOrder

| Context                                     | EntityCount |                                          Mean |       Error |      StdDev |      Median |             Ratio | RatioSD |  Allocated |     Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|------------:|------------:|------------:|------------------:|--------:|-----------:|----------------:|
| Arch                                        | 100000      |                                    8,819.9 μs |   733.73 μs | 2,140.32 μs |  8,271.5 μs |          baseline |         |     1072 B |                 |
| DefaultECS                                  | 100000      |                                   10,694.3 μs |   350.95 μs | 1,018.16 μs | 10,671.5 μs |      1.28x slower |   0.30x |      400 B |      2.68x less |
| DragonECS                                   | 100000      |                                    3,963.4 μs |    79.19 μs |   224.65 μs |  3,956.7 μs |      2.23x faster |   0.55x |      400 B |      2.68x less |
| Fennecs                                     | 100000      |                                   47,672.0 μs |   743.06 μs |   658.70 μs | 47,587.2 μs |      5.69x slower |   1.21x | 49600400 B | 46,269.03x more |
| FlecsNET                                    | 100000      |                                   88,778.0 μs | 1,236.98 μs | 1,157.08 μs | 88,676.0 μs |     10.60x slower |   2.26x |      400 B |      2.68x less |
| Friflo                                      | 100000      |                                    4,064.5 μs |   270.73 μs |   772.41 μs |  3,850.2 μs |      2.24x faster |   0.67x |      400 B |      2.68x less |
| LeoEcs                                      | 100000      |                                   16,779.7 μs |   329.41 μs |   541.22 μs | 16,784.5 μs |      2.00x slower |   0.43x |      400 B |      2.68x less |
| LeoEcsLite                                  | 100000      |                                    4,475.2 μs |   260.84 μs |   765.01 μs |  4,333.5 μs |      2.03x faster |   0.59x |      400 B |      2.68x less |
| Morpeh                                      | 100000      |                                   15,792.6 μs |   304.99 μs |   407.15 μs | 15,791.4 μs |      1.89x slower |   0.40x |      400 B |      2.68x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**836.5 μs** |    27.43 μs |    77.37 μs |    827.8 μs | **10.63x faster** |   2.75x |      400 B |      2.68x less |
| TinyEcs                                     | 100000      |                                    7,417.8 μs |   203.29 μs |   576.69 μs |  7,417.6 μs |      1.20x faster |   0.30x |  2400400 B |  2,239.18x more |
| Xeno                                        | 100000      |                                    6,259.2 μs |   389.00 μs | 1,109.85 μs |  6,148.6 μs |      1.45x faster |   0.43x |      400 B |      2.68x less |

# Add1Component

| Context                                     | EntityCount |                                          Mean |     Error |    StdDev |             Ratio | RatioSD |  Allocated |     Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|----------:|----------:|------------------:|--------:|-----------:|----------------:|
| Arch                                        | 100000      |                                    3,726.4 μs |  31.68 μs |  28.08 μs |          baseline |         |     1072 B |                 |
| DefaultECS                                  | 100000      |                                    1,329.7 μs |  12.90 μs |  11.44 μs |      2.80x faster |   0.03x |      400 B |      2.68x less |
| DragonECS                                   | 100000      |                                      841.0 μs |  16.69 μs |  38.34 μs |      4.44x faster |   0.20x |      400 B |      2.68x less |
| Fennecs                                     | 100000      |                                   34,711.3 μs | 501.54 μs | 444.60 μs |      9.32x slower |   0.13x | 49600400 B | 46,269.03x more |
| FlecsNET                                    | 100000      |                                   63,623.6 μs | 905.04 μs | 846.58 μs |     17.07x slower |   0.25x |      400 B |      2.68x less |
| Friflo                                      | 100000      |                                    1,530.2 μs |  17.34 μs |  23.15 μs |      2.44x faster |   0.04x |      400 B |      2.68x less |
| LeoEcs                                      | 100000      |                                    3,354.0 μs |  66.93 μs |  82.19 μs |      1.11x faster |   0.03x |      400 B |      2.68x less |
| LeoEcsLite                                  | 100000      |                                    1,508.7 μs |  27.42 μs |  56.64 μs |      2.47x faster |   0.08x |      400 B |      2.68x less |
| Morpeh                                      | 100000      |                                    2,595.7 μs |  54.54 μs | 157.37 μs |      1.44x faster |   0.08x |      400 B |      2.68x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**330.1 μs** |   6.24 μs |   5.53 μs | **11.29x faster** |   0.20x |      400 B |      2.68x less |
| TinyEcs                                     | 100000      |                                    3,705.5 μs |  72.58 μs |  96.90 μs |      1.01x faster |   0.03x |  2400400 B |  2,239.18x more |
| Xeno                                        | 100000      |                                    2,431.7 μs |  18.08 μs |  36.10 μs |      1.53x faster |   0.02x |      400 B |      2.68x less |

# Add1RandomComponentRandomOrder

| Context                                     | EntityCount |                                          Mean |    Error |   StdDev |            Ratio | RatioSD | Allocated | Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|---------:|---------:|-----------------:|--------:|----------:|------------:|
| Arch                                        | 100000      |                                      16.99 ms | 0.337 ms | 0.762 ms |         baseline |         |   2.29 MB |             |
| DefaultECS                                  | 100000      |                                      21.66 ms | 0.432 ms | 0.562 ms |     1.28x slower |   0.07x |   2.29 MB |  1.00x less |
| DragonECS                                   | 100000      |                                      17.94 ms | 0.159 ms | 0.149 ms |     1.06x slower |   0.05x |   2.29 MB |  1.00x less |
| Fennecs                                     | 100000      |                                      58.77 ms | 0.653 ms | 0.579 ms |     3.47x slower |   0.16x |  50.35 MB | 21.99x more |
| FlecsNET                                    | 100000      |                                     140.99 ms | 1.141 ms | 1.068 ms |     8.31x slower |   0.38x |   3.05 MB |  1.33x more |
| Friflo                                      | 100000      |                                      14.34 ms | 0.379 ms | 1.094 ms |     1.19x faster |   0.10x |   3.05 MB |  1.33x more |
| LeoEcs                                      | 100000      |                                      31.24 ms | 0.592 ms | 1.298 ms |     1.84x slower |   0.11x |   3.05 MB |  1.33x more |
| LeoEcsLite                                  | 100000      |                                      17.56 ms | 0.344 ms | 0.536 ms |     1.04x slower |   0.06x |   2.29 MB |  1.00x less |
| Morpeh                                      | 100000      |                                      28.49 ms | 0.287 ms | 0.268 ms |     1.68x slower |   0.08x |   2.29 MB |  1.00x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**10.83 ms** | 0.289 ms | 0.840 ms | **1.58x faster** |   0.14x |   2.29 MB |  1.00x less |
| TinyEcs                                     | 100000      |                                      28.64 ms | 0.552 ms | 0.461 ms |     1.69x slower |   0.08x |   5.34 MB |  2.33x more |
| Xeno                                        | 100000      |                                      21.20 ms | 0.422 ms | 0.934 ms |     1.25x slower |   0.08x |   3.79 MB |  1.66x more |

# Add1RandomComponent

| Context                                     | EntityCount |                                          Mean |     Error |    StdDev |     Median |            Ratio | RatioSD | Allocated | Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|----------:|----------:|-----------:|-----------------:|--------:|----------:|------------:|
| Arch                                        | 100000      |                                     10.472 ms | 0.0598 ms | 0.0499 ms |  10.470 ms |         baseline |         |   2.29 MB |             |
| DefaultECS                                  | 100000      |                                      7.786 ms | 0.0245 ms | 0.0229 ms |   7.777 ms |     1.35x faster |   0.01x |   2.29 MB |  1.00x less |
| DragonECS                                   | 100000      |                                      7.947 ms | 0.1566 ms | 0.2573 ms |   7.806 ms |     1.32x faster |   0.04x |   2.29 MB |  1.00x less |
| Fennecs                                     | 100000      |                                     44.789 ms | 0.4027 ms | 0.3570 ms |  44.864 ms |     4.28x slower |   0.04x |  50.35 MB | 21.99x more |
| FlecsNET                                    | 100000      |                                    123.506 ms | 0.4971 ms | 0.4151 ms | 123.327 ms |    11.79x slower |   0.07x |   3.05 MB |  1.33x more |
| Friflo                                      | 100000      |                                      9.510 ms | 0.1167 ms | 0.2193 ms |   9.461 ms |     1.10x faster |   0.02x |   3.05 MB |  1.33x more |
| LeoEcs                                      | 100000      |                                     13.387 ms | 0.2617 ms | 0.4584 ms |  13.438 ms |     1.28x slower |   0.04x |   3.05 MB |  1.33x more |
| LeoEcsLite                                  | 100000      |                                      9.186 ms | 0.0663 ms | 0.1071 ms |   9.183 ms |     1.14x faster |   0.01x |   2.29 MB |  1.00x less |
| Morpeh                                      | 100000      |                                     10.488 ms | 0.2082 ms | 0.1846 ms |  10.414 ms |     1.00x slower |   0.02x |   2.29 MB |  1.00x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**7.694 ms** | 0.0604 ms | 0.0472 ms |   7.679 ms | **1.36x faster** |   0.01x |   2.29 MB |  1.00x less |
| TinyEcs                                     | 100000      |                                     19.178 ms | 0.1727 ms | 0.1531 ms |  19.190 ms |     1.83x slower |   0.02x |   5.34 MB |  2.33x more |
| Xeno                                        | 100000      |                                     13.134 ms | 0.2459 ms | 0.2053 ms |  13.039 ms |     1.25x slower |   0.02x |   3.79 MB |  1.66x more |

# Add2ComponentsRandomOrder

| Context                                     | EntityCount |                                          Mean |     Error |    StdDev |     Median |            Ratio | RatioSD |   Allocated |      Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|----------:|----------:|-----------:|-----------------:|--------:|------------:|-----------------:|
| Arch                                        | 100000      |                                      8.630 ms | 0.2693 ms | 0.7856 ms |   8.517 ms |         baseline |         |      1072 B |                  |
| DefaultECS                                  | 100000      |                                     13.926 ms | 0.3093 ms | 0.8874 ms |  13.882 ms |     1.63x slower |   0.17x |       400 B |       2.68x less |
| DragonECS                                   | 100000      |                                      6.236 ms | 0.1237 ms | 0.3236 ms |   6.184 ms |     1.39x faster |   0.14x |       400 B |       2.68x less |
| Fennecs                                     | 100000      |                                     95.385 ms | 0.8471 ms | 0.7509 ms |  95.012 ms |    11.14x slower |   0.96x | 113600400 B | 105,970.52x more |
| FlecsNET                                    | 100000      |                                    152.545 ms | 1.3933 ms | 1.2351 ms | 152.754 ms |    17.82x slower |   1.54x |       400 B |       2.68x less |
| Friflo                                      | 100000      |                                      6.722 ms | 0.3005 ms | 0.8719 ms |   6.742 ms |     1.30x faster |   0.20x |       400 B |       2.68x less |
| LeoEcs                                      | 100000      |                                     20.039 ms | 0.3986 ms | 0.5456 ms |  19.999 ms |     2.34x slower |   0.21x |       400 B |       2.68x less |
| LeoEcsLite                                  | 100000      |                                      7.500 ms | 0.3134 ms | 0.9141 ms |   7.502 ms |     1.17x faster |   0.18x |       400 B |       2.68x less |
| Morpeh                                      | 100000      |                                     24.156 ms | 0.4837 ms | 1.3800 ms |  23.650 ms |     2.82x slower |   0.29x |       400 B |       2.68x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**1.480 ms** | 0.0389 ms | 0.1098 ms |   1.456 ms | **5.86x faster** |   0.67x |       400 B |       2.68x less |
| TinyEcs                                     | 100000      |                                     17.197 ms | 0.2734 ms | 0.2807 ms |  17.181 ms |     2.01x slower |   0.18x |   4800400 B |   4,477.99x more |
| Xeno                                        | 100000      |                                      8.055 ms | 0.3602 ms | 1.0277 ms |   7.950 ms |     1.09x faster |   0.17x |   1045224 B |     975.02x more |

# Add2Components

| Context                                     | EntityCount |                                          Mean |     Error |    StdDev |            Ratio | RatioSD |   Allocated |      Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|----------:|----------:|-----------------:|--------:|------------:|-----------------:|
| Arch                                        | 100000      |                                    5,258.7 μs |  52.62 μs |  46.65 μs |         baseline |         |      1072 B |                  |
| DefaultECS                                  | 100000      |                                    2,629.8 μs |  40.04 μs |  37.46 μs |     2.00x faster |   0.03x |       400 B |       2.68x less |
| DragonECS                                   | 100000      |                                    1,326.4 μs |  22.08 μs |  18.44 μs |     3.97x faster |   0.06x |       400 B |       2.68x less |
| Fennecs                                     | 100000      |                                   78,989.4 μs | 392.64 μs | 327.87 μs |    15.02x slower |   0.14x | 113600400 B | 105,970.52x more |
| FlecsNET                                    | 100000      |                                  136,841.1 μs | 738.66 μs | 690.94 μs |    26.02x slower |   0.26x |       400 B |       2.68x less |
| Friflo                                      | 100000      |                                    3,472.7 μs |  63.32 μs |  59.23 μs |     1.51x faster |   0.03x |       400 B |       2.68x less |
| LeoEcs                                      | 100000      |                                    5,498.1 μs |  24.76 μs |  21.95 μs |     1.05x slower |   0.01x |       400 B |       2.68x less |
| LeoEcsLite                                  | 100000      |                                    3,235.1 μs |  32.41 μs |  31.83 μs |     1.63x faster |   0.02x |       400 B |       2.68x less |
| Morpeh                                      | 100000      |                                    3,174.7 μs |  56.18 μs |  73.05 μs |     1.66x faster |   0.04x |       400 B |       2.68x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**672.8 μs** |   3.37 μs |   2.98 μs | **7.82x faster** |   0.07x |       400 B |       2.68x less |
| TinyEcs                                     | 100000      |                                    6,982.9 μs | 121.55 μs | 107.75 μs |     1.33x slower |   0.02x |   4800400 B |   4,477.99x more |
| Xeno                                        | 100000      |                                    3,090.2 μs |  34.97 μs |  67.37 μs |     1.70x faster |   0.04x |   1045224 B |     975.02x more |

# Add2RandomComponentsRandomOrder

| Context                                     | EntityCount |                                          Mean |    Error |   StdDev |            Ratio | RatioSD | Allocated | Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|---------:|---------:|-----------------:|--------:|----------:|------------:|
| Arch                                        | 100000      |                                      19.74 ms | 0.424 ms | 1.230 ms |         baseline |         |   2.29 MB |             |
| DefaultECS                                  | 100000      |                                      23.84 ms | 0.472 ms | 0.677 ms |     1.21x slower |   0.08x |   2.29 MB |  1.00x less |
| DragonECS                                   | 100000      |                                      18.75 ms | 0.330 ms | 0.292 ms |     1.05x faster |   0.07x |   2.29 MB |  1.00x less |
| Fennecs                                     | 100000      |                                     108.64 ms | 0.496 ms | 0.440 ms |     5.52x slower |   0.34x | 111.39 MB | 48.65x more |
| FlecsNET                                    | 100000      |                                     166.72 ms | 1.084 ms | 0.961 ms |     8.48x slower |   0.52x |   3.05 MB |  1.33x more |
| Friflo                                      | 100000      |                                      15.85 ms | 0.417 ms | 1.217 ms |     1.25x faster |   0.12x |   3.05 MB |  1.33x more |
| LeoEcs                                      | 100000      |                                      34.31 ms | 0.650 ms | 0.667 ms |     1.74x slower |   0.11x |   3.05 MB |  1.33x more |
| LeoEcsLite                                  | 100000      |                                      25.64 ms | 0.480 ms | 1.063 ms |     1.30x slower |   0.10x |   2.29 MB |  1.00x less |
| Morpeh                                      | 100000      |                                      31.92 ms | 0.517 ms | 0.484 ms |     1.62x slower |   0.10x |   2.29 MB |  1.00x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**11.71 ms** | 0.566 ms | 1.624 ms | **1.72x faster** |   0.25x |   2.29 MB |  1.00x less |
| TinyEcs                                     | 100000      |                                      33.06 ms | 0.254 ms | 0.212 ms |     1.68x slower |   0.10x |   7.63 MB |  3.33x more |
| Xeno                                        | 100000      |                                      23.07 ms | 0.458 ms | 0.936 ms |     1.17x slower |   0.09x |   4.29 MB |  1.88x more |

# Add2RandomComponents

| Context                                     | EntityCount |                                          Mean |     Error |    StdDev |            Ratio | RatioSD | Allocated | Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|----------:|----------:|-----------------:|--------:|----------:|------------:|
| Arch                                        | 100000      |                                     13.373 ms | 0.0713 ms | 0.0632 ms |         baseline |         |   2.29 MB |             |
| DefaultECS                                  | 100000      |                                      9.712 ms | 0.0588 ms | 0.0522 ms |     1.38x faster |   0.01x |   2.29 MB |  1.00x less |
| DragonECS                                   | 100000      |                                      8.564 ms | 0.0652 ms | 0.1108 ms |     1.56x faster |   0.02x |   2.29 MB |  1.00x less |
| Fennecs                                     | 100000      |                                     91.379 ms | 0.5319 ms | 0.4976 ms |     6.83x slower |   0.05x | 111.39 MB | 48.65x more |
| FlecsNET                                    | 100000      |                                    157.655 ms | 1.1228 ms | 1.0503 ms |    11.79x slower |   0.09x |   3.05 MB |  1.33x more |
| Friflo                                      | 100000      |                                     12.057 ms | 0.1607 ms | 0.1342 ms |     1.11x faster |   0.01x |   3.05 MB |  1.33x more |
| LeoEcs                                      | 100000      |                                     17.575 ms | 0.3364 ms | 0.4005 ms |     1.31x slower |   0.03x |   3.05 MB |  1.33x more |
| LeoEcsLite                                  | 100000      |                                     13.163 ms | 0.1396 ms | 0.1714 ms |     1.02x faster |   0.01x |   2.29 MB |  1.00x less |
| Morpeh                                      | 100000      |                                     12.213 ms | 0.1716 ms | 0.1521 ms |     1.10x faster |   0.01x |   2.29 MB |  1.00x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**7.793 ms** | 0.1104 ms | 0.0922 ms | **1.72x faster** |   0.02x |   2.29 MB |  1.00x less |
| TinyEcs                                     | 100000      |                                     32.669 ms | 0.3236 ms | 0.2703 ms |     2.44x slower |   0.02x |   7.63 MB |  3.33x more |
| Xeno                                        | 100000      |                                     12.346 ms | 0.2259 ms | 0.1764 ms |     1.08x faster |   0.02x |   4.29 MB |  1.88x more |

# Add3ComponentsRandomOrder

| Context                                     | EntityCount |                                          Mean |     Error |    StdDev |     Median |            Ratio | RatioSD |   Allocated |      Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|----------:|----------:|-----------:|-----------------:|--------:|------------:|-----------------:|
| Arch                                        | 100000      |                                      9.575 ms | 0.2476 ms | 0.7223 ms |   9.617 ms |         baseline |         |      1072 B |                  |
| DefaultECS                                  | 100000      |                                     17.971 ms | 0.3567 ms | 0.8949 ms |  17.989 ms |     1.89x slower |   0.17x |       400 B |       2.68x less |
| DragonECS                                   | 100000      |                                     10.576 ms | 0.2109 ms | 0.3345 ms |  10.619 ms |     1.11x slower |   0.09x |       400 B |       2.68x less |
| Fennecs                                     | 100000      |                                    151.817 ms | 1.8379 ms | 1.5348 ms | 151.827 ms |    15.94x slower |   1.20x | 184800400 B | 172,388.43x more |
| FlecsNET                                    | 100000      |                                    268.794 ms | 4.4368 ms | 3.9331 ms | 270.525 ms |    28.23x slower |   2.15x |       400 B |       2.68x less |
| Friflo                                      | 100000      |                                      9.653 ms | 0.3152 ms | 0.8786 ms |   9.626 ms |     1.01x slower |   0.12x |       400 B |       2.68x less |
| LeoEcs                                      | 100000      |                                     23.120 ms | 0.3496 ms | 0.3099 ms |  23.089 ms |     2.43x slower |   0.18x |       400 B |       2.68x less |
| LeoEcsLite                                  | 100000      |                                     11.705 ms | 0.3307 ms | 0.9218 ms |  11.617 ms |     1.23x slower |   0.13x |       400 B |       2.68x less |
| Morpeh                                      | 100000      |                                     25.053 ms | 0.3557 ms | 0.3154 ms |  25.031 ms |     2.63x slower |   0.20x |       400 B |       2.68x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**1.993 ms** | 0.0581 ms | 0.1600 ms |   1.948 ms | **4.83x faster** |   0.51x |       400 B |       2.68x less |
| TinyEcs                                     | 100000      |                                     20.767 ms | 0.3865 ms | 0.6970 ms |  20.688 ms |     2.18x slower |   0.18x |   7200400 B |   6,716.79x more |
| Xeno                                        | 100000      |                                     10.749 ms | 0.5108 ms | 1.4655 ms |  10.480 ms |     1.13x slower |   0.18x |   1045232 B |     975.03x more |

# Add3Components

| Context                                     | EntityCount |                                          Mean |     Error |    StdDev |            Ratio | RatioSD |   Allocated |      Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|----------:|----------:|-----------------:|--------:|------------:|-----------------:|
| Arch                                        | 100000      |                                      5.621 ms | 0.0772 ms | 0.0645 ms |         baseline |         |      1072 B |                  |
| DefaultECS                                  | 100000      |                                      4.177 ms | 0.0333 ms | 0.0312 ms |     1.35x faster |   0.02x |       400 B |       2.68x less |
| DragonECS                                   | 100000      |                                      2.040 ms | 0.0276 ms | 0.0231 ms |     2.75x faster |   0.04x |       400 B |       2.68x less |
| Fennecs                                     | 100000      |                                    140.758 ms | 0.8871 ms | 0.8298 ms |    25.05x slower |   0.31x | 184800400 B | 172,388.43x more |
| FlecsNET                                    | 100000      |                                    227.358 ms | 1.5572 ms | 1.4566 ms |    40.46x slower |   0.51x |       400 B |       2.68x less |
| Friflo                                      | 100000      |                                      6.111 ms | 0.0212 ms | 0.0283 ms |     1.09x slower |   0.01x |       400 B |       2.68x less |
| LeoEcs                                      | 100000      |                                      8.074 ms | 0.0718 ms | 0.0672 ms |     1.44x slower |   0.02x |       400 B |       2.68x less |
| LeoEcsLite                                  | 100000      |                                      5.825 ms | 0.0464 ms | 0.0434 ms |     1.04x slower |   0.01x |       400 B |       2.68x less |
| Morpeh                                      | 100000      |                                      3.974 ms | 0.0552 ms | 0.0489 ms |     1.41x faster |   0.02x |       400 B |       2.68x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**1.082 ms** | 0.0102 ms | 0.0096 ms | **5.19x faster** |   0.07x |       400 B |       2.68x less |
| TinyEcs                                     | 100000      |                                     19.667 ms | 0.3004 ms | 0.2346 ms |     3.50x slower |   0.06x |   7200400 B |   6,716.79x more |
| Xeno                                        | 100000      |                                      3.683 ms | 0.0355 ms | 0.0658 ms |     1.53x faster |   0.03x |   1045232 B |     975.03x more |

# Add3RandomComponentsRandomOrder

| Context                                     | EntityCount |                                          Mean |    Error |   StdDev |    Median |            Ratio | RatioSD | Allocated | Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|---------:|---------:|----------:|-----------------:|--------:|----------:|------------:|
| Arch                                        | 100000      |                                      19.36 ms | 0.493 ms | 1.333 ms |  18.92 ms |         baseline |         |   2.29 MB |             |
| DefaultECS                                  | 100000      |                                      27.44 ms | 0.498 ms | 0.817 ms |  27.43 ms |     1.42x slower |   0.10x |   2.29 MB |  1.00x less |
| DragonECS                                   | 100000      |                                      20.44 ms | 0.398 ms | 0.458 ms |  20.48 ms |     1.06x slower |   0.07x |   2.29 MB |  1.00x less |
| Fennecs                                     | 100000      |                                     163.52 ms | 0.914 ms | 0.763 ms | 163.61 ms |     8.48x slower |   0.53x | 179.29 MB | 78.30x more |
| FlecsNET                                    | 100000      |                                     328.14 ms | 1.104 ms | 1.032 ms | 327.95 ms |    17.02x slower |   1.06x |   3.05 MB |  1.33x more |
| Friflo                                      | 100000      |                                      18.87 ms | 0.372 ms | 0.398 ms |  18.79 ms |     1.03x faster |   0.07x |   3.05 MB |  1.33x more |
| LeoEcs                                      | 100000      |                                      42.37 ms | 0.711 ms | 0.760 ms |  42.39 ms |     2.20x slower |   0.14x |   3.05 MB |  1.33x more |
| LeoEcsLite                                  | 100000      |                                      32.16 ms | 0.634 ms | 0.623 ms |  32.24 ms |     1.67x slower |   0.11x |   2.29 MB |  1.00x less |
| Morpeh                                      | 100000      |                                      34.84 ms | 0.332 ms | 0.277 ms |  34.80 ms |     1.81x slower |   0.11x |   2.29 MB |  1.00x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**10.77 ms** | 0.229 ms | 0.658 ms |  10.76 ms | **1.80x faster** |   0.16x |   2.29 MB |  1.00x less |
| TinyEcs                                     | 100000      |                                      49.42 ms | 1.450 ms | 4.252 ms |  51.38 ms |     2.56x slower |   0.27x |   9.92 MB |  4.33x more |
| Xeno                                        | 100000      |                                      23.61 ms | 0.470 ms | 1.248 ms |  23.56 ms |     1.22x slower |   0.10x |   4.05 MB |  1.77x more |

# Add3RandomComponents

| Context                                     | EntityCount |                                          Mean |     Error |    StdDev |            Ratio | RatioSD | Allocated | Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|----------:|----------:|-----------------:|--------:|----------:|------------:|
| Arch                                        | 100000      |                                     13.132 ms | 0.0569 ms | 0.0444 ms |         baseline |         |   2.29 MB |             |
| DefaultECS                                  | 100000      |                                     11.240 ms | 0.1144 ms | 0.1070 ms |     1.17x faster |   0.01x |   2.29 MB |  1.00x less |
| DragonECS                                   | 100000      |                                      9.558 ms | 0.1012 ms | 0.1242 ms |     1.37x faster |   0.02x |   2.29 MB |  1.00x less |
| Fennecs                                     | 100000      |                                    145.696 ms | 1.5052 ms | 1.3343 ms |    11.09x slower |   0.10x | 179.29 MB | 78.30x more |
| FlecsNET                                    | 100000      |                                    308.592 ms | 1.0801 ms | 0.9019 ms |    23.50x slower |   0.10x |   3.05 MB |  1.33x more |
| Friflo                                      | 100000      |                                     15.382 ms | 0.0358 ms | 0.0317 ms |     1.17x slower |   0.00x |   3.05 MB |  1.33x more |
| LeoEcs                                      | 100000      |                                     22.660 ms | 0.4324 ms | 0.4627 ms |     1.73x slower |   0.03x |   4.05 MB |  1.77x more |
| LeoEcsLite                                  | 100000      |                                     18.484 ms | 0.1257 ms | 0.1049 ms |     1.41x slower |   0.01x |   2.29 MB |  1.00x less |
| Morpeh                                      | 100000      |                                     12.797 ms | 0.1782 ms | 0.1579 ms |     1.03x faster |   0.01x |   2.29 MB |  1.00x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**8.902 ms** | 0.1414 ms | 0.1254 ms | **1.48x faster** |   0.02x |   2.29 MB |  1.00x less |
| TinyEcs                                     | 100000      |                                     38.137 ms | 0.7203 ms | 0.7397 ms |     2.90x slower |   0.06x |   9.92 MB |  4.33x more |
| Xeno                                        | 100000      |                                     12.767 ms | 0.1689 ms | 0.3526 ms |     1.03x faster |   0.03x |  68.05 MB | 29.72x more |

# Add4ComponentsRandomOrder

| Context                                     | EntityCount |                                          Mean |     Error |    StdDev |     Median |            Ratio | RatioSD |   Allocated |      Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|----------:|----------:|-----------:|-----------------:|--------:|------------:|-----------------:|
| Arch                                        | 100000      |                                      9.910 ms | 0.3224 ms | 0.9454 ms |  10.089 ms |         baseline |         |      1072 B |                  |
| DefaultECS                                  | 100000      |                                     20.984 ms | 0.4126 ms | 1.0427 ms |  20.823 ms |     2.14x slower |   0.23x |       400 B |       2.68x less |
| DragonECS                                   | 100000      |                                     12.755 ms | 0.2324 ms | 0.1814 ms |  12.703 ms |     1.30x slower |   0.13x |       400 B |       2.68x less |
| Fennecs                                     | 100000      |                                    219.496 ms | 1.7198 ms | 1.4361 ms | 219.592 ms |    22.35x slower |   2.16x | 268800400 B | 250,746.64x more |
| FlecsNET                                    | 100000      |                                    341.030 ms | 0.4741 ms | 0.4203 ms | 340.907 ms |    34.73x slower |   3.35x |       400 B |       2.68x less |
| Friflo                                      | 100000      |                                     12.089 ms | 0.3785 ms | 1.1099 ms |  11.678 ms |     1.23x slower |   0.16x |       400 B |       2.68x less |
| LeoEcs                                      | 100000      |                                     25.780 ms | 0.4997 ms | 0.4907 ms |  25.926 ms |     2.63x slower |   0.26x |       400 B |       2.68x less |
| LeoEcsLite                                  | 100000      |                                     16.004 ms | 0.4683 ms | 1.3662 ms |  15.770 ms |     1.63x slower |   0.21x |       400 B |       2.68x less |
| Morpeh                                      | 100000      |                                     29.748 ms | 0.5777 ms | 0.6421 ms |  29.738 ms |     3.03x slower |   0.30x |       400 B |       2.68x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**3.538 ms** | 0.2983 ms | 0.8413 ms |   3.240 ms | **2.94x faster** |   0.65x |       400 B |       2.68x less |
| TinyEcs                                     | 100000      |                                     24.546 ms | 0.4822 ms | 0.7069 ms |  24.347 ms |     2.50x slower |   0.25x |   9600400 B |   8,955.60x more |
| Xeno                                        | 100000      |                                     13.221 ms | 0.3069 ms | 0.8556 ms |  13.249 ms |     1.35x slower |   0.16x |   1045232 B |     975.03x more |

# Add4Components

| Context                                     | EntityCount |                                          Mean |     Error |    StdDev |            Ratio | RatioSD |   Allocated |      Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|----------:|----------:|-----------------:|--------:|------------:|-----------------:|
| Arch                                        | 100000      |                                      5.875 ms | 0.0525 ms | 0.0438 ms |         baseline |         |      1072 B |                  |
| DefaultECS                                  | 100000      |                                      5.160 ms | 0.0385 ms | 0.0341 ms |     1.14x faster |   0.01x |       400 B |       2.68x less |
| DragonECS                                   | 100000      |                                      2.745 ms | 0.0247 ms | 0.0219 ms |     2.14x faster |   0.02x |       400 B |       2.68x less |
| Fennecs                                     | 100000      |                                    195.604 ms | 0.8188 ms | 0.7659 ms |    33.30x slower |   0.27x | 268800400 B | 250,746.64x more |
| FlecsNET                                    | 100000      |                                    295.940 ms | 2.5707 ms | 2.4046 ms |    50.38x slower |   0.54x |       400 B |       2.68x less |
| Friflo                                      | 100000      |                                      8.150 ms | 0.0582 ms | 0.0544 ms |     1.39x slower |   0.01x |       400 B |       2.68x less |
| LeoEcs                                      | 100000      |                                     12.153 ms | 0.1866 ms | 0.1457 ms |     2.07x slower |   0.03x |       400 B |       2.68x less |
| LeoEcsLite                                  | 100000      |                                      8.535 ms | 0.0682 ms | 0.0570 ms |     1.45x slower |   0.01x |       400 B |       2.68x less |
| Morpeh                                      | 100000      |                                      5.072 ms | 0.0603 ms | 0.0564 ms |     1.16x faster |   0.01x |       400 B |       2.68x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**1.438 ms** | 0.0284 ms | 0.0291 ms | **4.09x faster** |   0.09x |       400 B |       2.68x less |
| TinyEcs                                     | 100000      |                                     24.338 ms | 0.3869 ms | 0.4300 ms |     4.14x slower |   0.08x |   9600400 B |   8,955.60x more |
| Xeno                                        | 100000      |                                      4.255 ms | 0.0585 ms | 0.0993 ms |     1.38x faster |   0.03x |   1045232 B |     975.03x more |

# CreateEmptyEntity

| Context                                     | EntityCount |                                          Mean |     Error |    StdDev |     Median |             Ratio | RatioSD | Allocated |    Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|----------:|----------:|-----------:|------------------:|--------:|----------:|---------------:|
| Arch                                        | 100000      |                                    3,622.0 μs |  71.45 μs |  87.74 μs | 3,640.1 μs |          baseline |         |    1072 B |                |
| DefaultECS                                  | 100000      |                                    1,323.6 μs |  14.99 μs |  12.51 μs | 1,321.0 μs |      2.74x faster |   0.07x |     400 B |     2.68x less |
| DragonECS                                   | 100000      |                                      608.6 μs |   9.97 μs |   8.33 μs |   608.5 μs |      5.95x faster |   0.16x |     400 B |     2.68x less |
| Fennecs                                     | 100000      |                                    1,805.8 μs |  27.57 μs |  24.44 μs | 1,793.3 μs |      2.01x faster |   0.05x |     400 B |     2.68x less |
| FlecsNET                                    | 100000      |                                    7,976.5 μs | 153.97 μs | 177.31 μs | 7,955.8 μs |      2.20x slower |   0.07x |     400 B |     2.68x less |
| Friflo                                      | 100000      |                                      958.2 μs |  18.82 μs |  16.68 μs |   952.0 μs |      3.78x faster |   0.11x |     400 B |     2.68x less |
| LeoEcs                                      | 100000      |                                      440.3 μs |   8.59 μs |  12.04 μs |   436.3 μs |      8.23x faster |   0.29x |     400 B |     2.68x less |
| LeoEcsLite                                  | 100000      |                                      890.3 μs |   6.11 μs |   5.10 μs |   890.6 μs |      4.07x faster |   0.10x |     400 B |     2.68x less |
| Morpeh                                      | 100000      | <span style="color: lightgreen;">**270.1 μs** |   4.15 μs |   6.58 μs |   269.8 μs |     13.42x faster |   0.45x |     400 B |     2.68x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  |      <span style="color: white;">**286.9 μs** |   5.33 μs |   4.98 μs |   284.1 μs | **12.63x faster** |   0.37x |     400 B |     2.68x less |
| TinyEcs                                     | 100000      |                                    2,976.5 μs |  59.44 μs | 131.72 μs | 2,920.4 μs |      1.22x faster |   0.06x | 3689200 B | 3,441.42x more |
| Xeno                                        | 100000      |                                    1,249.8 μs |   4.98 μs |   4.16 μs | 1,248.8 μs |      2.90x faster |   0.07x |     400 B |     2.68x less |

# CreateEntityWith1Component

| Context                                     | EntityCount |                                          Mean |     Error |    StdDev |      Median |            Ratio | RatioSD |  Allocated |     Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|----------:|----------:|------------:|-----------------:|--------:|-----------:|----------------:|
| Arch                                        | 100000      |                                    2,771.5 μs |  50.94 μs |  45.16 μs |  2,759.3 μs |         baseline |         |     1072 B |                 |
| DefaultECS                                  | 100000      |                                    3,088.1 μs |  24.33 μs |  20.31 μs |  3,089.6 μs |     1.11x slower |   0.02x |      400 B |      2.68x less |
| DragonECS                                   | 100000      |                                    1,962.8 μs |  76.96 μs | 220.82 μs |  1,849.0 μs |     1.43x faster |   0.14x |      400 B |      2.68x less |
| Fennecs                                     | 100000      |                                   36,525.6 μs | 693.75 μs | 771.10 μs | 36,403.8 μs |    13.18x slower |   0.34x | 49600400 B | 46,269.03x more |
| FlecsNET                                    | 100000      |                                   71,564.6 μs | 548.15 μs | 485.92 μs | 71,551.6 μs |    25.83x slower |   0.43x |      400 B |      2.68x less |
| Friflo                                      | 100000      |                                    3,052.4 μs |  20.94 μs |  18.56 μs |  3,054.4 μs |     1.10x slower |   0.02x |      400 B |      2.68x less |
| LeoEcs                                      | 100000      |                                    3,817.7 μs |  63.50 μs |  59.39 μs |  3,818.7 μs |     1.38x slower |   0.03x |      400 B |      2.68x less |
| LeoEcsLite                                  | 100000      |                                    2,011.2 μs | 118.28 μs | 313.66 μs |  1,828.3 μs |     1.41x faster |   0.18x |      400 B |      2.68x less |
| Morpeh                                      | 100000      |                                    2,845.9 μs |  27.61 μs |  23.06 μs |  2,841.3 μs |     1.03x slower |   0.02x |      400 B |      2.68x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**611.6 μs** |  59.18 μs | 173.58 μs |    499.3 μs | **4.84x faster** |   1.13x |    12736 B |     11.88x more |
| TinyEcs                                     | 100000      |                                    6,379.5 μs | 127.12 μs | 321.25 μs |  6,366.6 μs |     2.30x slower |   0.12x |  5941648 B |  5,542.58x more |
| Xeno                                        | 100000      |                                    3,721.9 μs |  51.19 μs |  45.38 μs |  3,718.6 μs |     1.34x slower |   0.03x |  1045160 B |    974.96x more |

# CreateEntityWith1RandomComponent

| Context                                     | EntityCount | ChunkSize |                                          Mean |     Error |    StdDev |     Median |            Ratio | RatioSD |  Allocated |      Alloc Ratio |
|---------------------------------------------|-------------|-----------|----------------------------------------------:|----------:|----------:|-----------:|-----------------:|--------:|-----------:|-----------------:|
| Arch                                        | 100000      | 1         |                                      8.955 ms | 0.5202 ms | 1.5337 ms |   9.111 ms |         baseline |         | 28822320 B |                  |
| DefaultECS                                  | 100000      | 1         |                                      6.282 ms | 0.1242 ms | 0.1037 ms |   6.241 ms |     1.43x faster |   0.24x |  3200400 B |      9.006x less |
| DragonECS                                   | 100000      | 1         |                                      6.732 ms | 0.7226 ms | 2.0849 ms |   5.670 ms |     1.42x faster |   0.39x |      400 B | 72,055.800x less |
| Fennecs                                     | 100000      | 1         |                                     39.894 ms | 0.7597 ms | 1.6516 ms |  39.159 ms |     4.60x slower |   0.92x | 49600400 B |      1.721x more |
| FlecsNET                                    | 100000      | 1         |                                    133.596 ms | 2.3591 ms | 1.8418 ms | 133.407 ms |    15.42x slower |   3.01x |      400 B | 72,055.800x less |
| Friflo                                      | 100000      | 1         |                                      5.529 ms | 0.0463 ms | 0.0569 ms |   5.511 ms |     1.62x faster |   0.28x |      400 B | 72,055.800x less |
| LeoEcs                                      | 100000      | 1         |                                     17.091 ms | 0.3413 ms | 0.7633 ms |  17.079 ms |     1.97x slower |   0.39x |  8800400 B |      3.275x less |
| LeoEcsLite                                  | 100000      | 1         |                                      8.076 ms | 1.2190 ms | 3.5171 ms |   6.035 ms |     1.28x faster |   0.46x |      400 B | 72,055.800x less |
| Morpeh                                      | 100000      | 1         |                                      7.511 ms | 0.1672 ms | 0.4578 ms |   7.559 ms |     1.20x faster |   0.22x |      400 B | 72,055.800x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | 1         | <span style="color: lightgreen;">**3.394 ms** | 0.1201 ms | 0.3426 ms |   3.349 ms | **2.66x faster** |   0.52x |      400 B | 72,055.800x less |
| TinyEcs                                     | 100000      | 1         |                                     16.911 ms | 0.1653 ms | 0.1624 ms |  16.864 ms |     1.95x slower |   0.38x |  6007232 B |      4.798x less |
| Xeno                                        | 100000      | 1         |                                      5.207 ms | 0.0961 ms | 0.1942 ms |   5.177 ms |     1.72x faster |   0.30x |      400 B | 72,055.800x less |
|                                             |             |           |                                               |           |           |            |                  |         |            |                  |
| Arch                                        | 100000      | 4         |                                      7.457 ms | 0.5188 ms | 1.5296 ms |   7.522 ms |         baseline |         | 28197664 B |                  |
| DefaultECS                                  | 100000      | 4         |                                      4.912 ms | 0.0949 ms | 0.1362 ms |   4.875 ms |     1.52x faster |   0.31x |  3200400 B |      8.811x less |
| DragonECS                                   | 100000      | 4         |                                      4.131 ms | 0.0834 ms | 0.2166 ms |   4.133 ms |     1.81x faster |   0.38x |      400 B | 70,494.160x less |
| Fennecs                                     | 100000      | 4         |                                     38.289 ms | 0.5600 ms | 0.4372 ms |  38.438 ms |     5.40x slower |   1.34x | 49600400 B |      1.759x more |
| FlecsNET                                    | 100000      | 4         |                                     97.974 ms | 0.8377 ms | 0.6540 ms |  98.115 ms |    13.81x slower |   3.42x |      400 B | 70,494.160x less |
| Friflo                                      | 100000      | 4         |                                      3.394 ms | 0.0653 ms | 0.1349 ms |   3.406 ms |     2.20x faster |   0.46x |      400 B | 70,494.160x less |
| LeoEcs                                      | 100000      | 4         |                                     14.057 ms | 0.2523 ms | 0.4352 ms |  13.905 ms |     1.98x slower |   0.49x |  8800400 B |      3.204x less |
| LeoEcsLite                                  | 100000      | 4         |                                      9.973 ms | 0.1422 ms | 0.1110 ms |  10.025 ms |     1.41x slower |   0.35x |      400 B | 70,494.160x less |
| Morpeh                                      | 100000      | 4         |                                      4.448 ms | 0.0956 ms | 0.2617 ms |   4.366 ms |     1.68x faster |   0.36x |      400 B | 70,494.160x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | 4         | <span style="color: lightgreen;">**1.898 ms** | 0.1269 ms | 0.3578 ms |   1.845 ms | **4.06x faster** |   1.09x |      400 B | 70,494.160x less |
| TinyEcs                                     | 100000      | 4         |                                      9.165 ms | 0.1818 ms | 0.3135 ms |   9.156 ms |     1.29x slower |   0.32x |  6007232 B |      4.694x less |
| Xeno                                        | 100000      | 4         |                                      2.895 ms | 0.0573 ms | 0.1396 ms |   2.850 ms |     2.58x faster |   0.54x |      400 B | 70,494.160x less |
|                                             |             |           |                                               |           |           |            |                  |         |            |                  |
| Arch                                        | 100000      | 32        |                                      7.111 ms | 0.5267 ms | 1.5531 ms |   7.224 ms |         baseline |         | 28985968 B |                  |
| DefaultECS                                  | 100000      | 32        |                                      4.553 ms | 0.0825 ms | 0.1333 ms |   4.507 ms |     1.56x faster |   0.34x |  3200400 B |      9.057x less |
| DragonECS                                   | 100000      | 32        |                                      3.841 ms | 0.1262 ms | 0.3325 ms |   3.682 ms |     1.86x faster |   0.43x |      400 B | 72,464.920x less |
| Fennecs                                     | 100000      | 32        |                                     36.373 ms | 0.4189 ms | 0.3498 ms |  36.405 ms |     5.42x slower |   1.43x | 49600400 B |      1.711x more |
| FlecsNET                                    | 100000      | 32        |                                     87.061 ms | 0.9572 ms | 0.7473 ms |  86.949 ms |    12.96x slower |   3.43x |      400 B | 72,464.920x less |
| Friflo                                      | 100000      | 32        |                                      2.698 ms | 0.0489 ms | 0.1253 ms |   2.637 ms |     2.64x faster |   0.59x |      400 B | 72,464.920x less |
| LeoEcs                                      | 100000      | 32        |                                     12.123 ms | 0.2212 ms | 0.3172 ms |  12.107 ms |     1.80x slower |   0.48x |  8800400 B |      3.294x less |
| LeoEcsLite                                  | 100000      | 32        |                                      5.929 ms | 1.2443 ms | 3.5900 ms |   3.418 ms |     1.62x faster |   0.80x |      400 B | 72,464.920x less |
| Morpeh                                      | 100000      | 32        |                                      3.301 ms | 0.0746 ms | 0.2056 ms |   3.246 ms |     2.16x faster |   0.49x |      400 B | 72,464.920x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | 32        | <span style="color: lightgreen;">**2.175 ms** | 0.2410 ms | 0.6875 ms |   2.198 ms | **3.66x faster** |   1.56x |      400 B | 72,464.920x less |
| TinyEcs                                     | 100000      | 32        |                                      6.450 ms | 0.0938 ms | 0.1592 ms |   6.408 ms |     1.10x faster |   0.24x |  6023616 B |      4.812x less |
| Xeno                                        | 100000      | 32        |                                      2.238 ms | 0.0444 ms | 0.0897 ms |   2.222 ms |     3.18x faster |   0.70x |      400 B | 72,464.920x less |

# CreateEntityWith2Components

| Context                                     | EntityCount |                                          Mean |       Error |      StdDev |       Median |            Ratio | RatioSD |   Allocated |      Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|------------:|------------:|-------------:|-----------------:|--------:|------------:|-----------------:|
| Arch                                        | 100000      |                                    2,861.9 μs |    39.03 μs |    34.60 μs |   2,854.6 μs |         baseline |         |      1072 B |                  |
| DefaultECS                                  | 100000      |                                    4,386.5 μs |    47.41 μs |    42.03 μs |   4,397.8 μs |     1.53x slower |   0.02x |        64 B |      16.75x less |
| DragonECS                                   | 100000      |                                    2,568.7 μs |    49.98 μs |    64.98 μs |   2,562.4 μs |     1.11x faster |   0.03x |       400 B |       2.68x less |
| Fennecs                                     | 100000      |                                   85,812.3 μs | 2,447.37 μs | 6,942.80 μs |  83,176.0 μs |    29.99x slower |   2.44x | 113600400 B | 105,970.52x more |
| FlecsNET                                    | 100000      |                                  175,642.5 μs | 2,133.60 μs | 1,781.66 μs | 175,734.9 μs |    61.38x slower |   0.93x |       400 B |       2.68x less |
| Friflo                                      | 100000      |                                    4,906.8 μs |    62.94 μs |    58.88 μs |   4,885.7 μs |     1.71x slower |   0.03x |       400 B |       2.68x less |
| LeoEcs                                      | 100000      |                                    6,011.4 μs |    74.07 μs |    65.66 μs |   5,998.3 μs |     2.10x slower |   0.03x |       400 B |       2.68x less |
| LeoEcsLite                                  | 100000      |                                    3,722.3 μs |    26.02 μs |    31.95 μs |   3,721.2 μs |     1.30x slower |   0.02x |       400 B |       2.68x less |
| Morpeh                                      | 100000      |                                    3,786.1 μs |   131.35 μs |   374.74 μs |   3,709.9 μs |     1.32x slower |   0.13x |       400 B |       2.68x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**833.9 μs** |    11.74 μs |    11.53 μs |     829.2 μs | **3.43x faster** |   0.06x |       400 B |       2.68x less |
| TinyEcs                                     | 100000      |                                   17,120.9 μs |   341.07 μs |   810.59 μs |  16,894.7 μs |     5.98x slower |   0.29x |   8341648 B |   7,781.39x more |
| Xeno                                        | 100000      |                                    2,653.2 μs |    58.41 μs |   152.84 μs |   2,611.6 μs |     1.08x faster |   0.06x |   1045160 B |     974.96x more |

# CreateEntityWith2RandomComponents

| Context                                     | EntityCount | ChunkSize |                                          Mean |     Error |    StdDev |     Median |            Ratio | RatioSD |   Allocated |      Alloc Ratio |
|---------------------------------------------|-------------|-----------|----------------------------------------------:|----------:|----------:|-----------:|-----------------:|--------:|------------:|-----------------:|
| Arch                                        | 100000      | 1         |                                     10.829 ms | 0.8711 ms | 2.5410 ms |  10.382 ms |         baseline |         |  37494728 B |                  |
| DefaultECS                                  | 100000      | 1         |                                      8.566 ms | 0.1631 ms | 0.3369 ms |   8.519 ms |     1.27x faster |   0.30x |   3200400 B |     11.716x less |
| DragonECS                                   | 100000      | 1         |                                      6.541 ms | 0.1023 ms | 0.2202 ms |   6.514 ms |     1.66x faster |   0.39x |       400 B | 93,736.820x less |
| Fennecs                                     | 100000      | 1         |                                     84.687 ms | 0.9305 ms | 0.7265 ms |  84.677 ms |     8.26x slower |   1.95x | 113600400 B |      3.030x more |
| FlecsNET                                    | 100000      | 1         |                                    238.034 ms | 2.2141 ms | 1.8489 ms | 237.452 ms |    23.22x slower |   5.49x |       400 B | 93,736.820x less |
| Friflo                                      | 100000      | 1         |                                      7.667 ms | 0.0620 ms | 0.0484 ms |   7.672 ms |     1.41x faster |   0.33x |       400 B | 93,736.820x less |
| LeoEcs                                      | 100000      | 1         |                                     22.795 ms | 0.4527 ms | 0.9450 ms |  22.761 ms |     2.22x slower |   0.53x |   8800400 B |      4.261x less |
| LeoEcsLite                                  | 100000      | 1         |                                      9.196 ms | 0.1816 ms | 0.3411 ms |   9.159 ms |     1.18x faster |   0.28x |       400 B | 93,736.820x less |
| Morpeh                                      | 100000      | 1         |                                      8.572 ms | 0.1636 ms | 0.1818 ms |   8.534 ms |     1.26x faster |   0.30x |       400 B | 93,736.820x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | 1         | <span style="color: lightgreen;">**3.675 ms** | 0.1483 ms | 0.4232 ms |   3.552 ms | **2.98x faster** |   0.77x |       400 B | 93,736.820x less |
| TinyEcs                                     | 100000      | 1         |                                     28.126 ms | 0.3113 ms | 0.3057 ms |  28.009 ms |     2.74x slower |   0.65x |   8407232 B |      4.460x less |
| Xeno                                        | 100000      | 1         |                                      5.990 ms | 0.1185 ms | 0.3183 ms |   6.037 ms |     1.81x faster |   0.43x |       400 B | 93,736.820x less |
|                                             |             |           |                                               |           |           |            |                  |         |             |                  |
| Arch                                        | 100000      | 4         |                                      8.882 ms | 0.7239 ms | 2.1345 ms |   8.870 ms |         baseline |         |  36889448 B |                  |
| DefaultECS                                  | 100000      | 4         |                                      6.608 ms | 0.1298 ms | 0.2273 ms |   6.580 ms |     1.35x faster |   0.33x |   3200400 B |     11.527x less |
| DragonECS                                   | 100000      | 4         |                                      5.606 ms | 0.6124 ms | 1.6868 ms |   4.867 ms |     1.68x faster |   0.52x |       400 B | 92,223.620x less |
| Fennecs                                     | 100000      | 4         |                                     81.715 ms | 1.5846 ms | 1.3232 ms |  81.528 ms |     9.78x slower |   2.56x | 113600400 B |      3.079x more |
| FlecsNET                                    | 100000      | 4         |                                    202.709 ms | 3.5561 ms | 3.9526 ms | 202.053 ms |    24.27x slower |   6.35x |       400 B | 92,223.620x less |
| Friflo                                      | 100000      | 4         |                                      5.847 ms | 0.0599 ms | 0.0589 ms |   5.859 ms |     1.52x faster |   0.36x |       400 B | 92,223.620x less |
| LeoEcs                                      | 100000      | 4         |                                     18.159 ms | 0.3509 ms | 0.4919 ms |  18.077 ms |     2.17x slower |   0.57x | 344345032 B |      9.335x more |
| LeoEcsLite                                  | 100000      | 4         |                                      7.773 ms | 0.1631 ms | 0.4325 ms |   7.717 ms |     1.15x faster |   0.28x |       400 B | 92,223.620x less |
| Morpeh                                      | 100000      | 4         |                                      5.431 ms | 0.1275 ms | 0.3532 ms |   5.415 ms |     1.64x faster |   0.41x |       400 B | 92,223.620x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | 4         | <span style="color: lightgreen;">**2.744 ms** | 0.1852 ms | 0.5283 ms |   2.687 ms | **3.36x faster** |   1.04x |       400 B | 92,223.620x less |
| TinyEcs                                     | 100000      | 4         |                                     19.667 ms | 0.2762 ms | 0.3781 ms |  19.589 ms |     2.35x slower |   0.62x |  75483328 B |      2.046x more |
| Xeno                                        | 100000      | 4         |                                      3.835 ms | 0.0862 ms | 0.2302 ms |   3.853 ms |     2.32x faster |   0.57x |       400 B | 92,223.620x less |
|                                             |             |           |                                               |           |           |            |                  |         |             |                  |
| Arch                                        | 100000      | 32        |                                      8.898 ms | 0.6712 ms | 1.9685 ms |   8.745 ms |         baseline |         |  37882832 B |                  |
| DefaultECS                                  | 100000      | 32        |                                      6.091 ms | 0.1175 ms | 0.1685 ms |   6.122 ms |     1.46x faster |   0.32x | 372299368 B |      9.828x more |
| DragonECS                                   | 100000      | 32        |                                      6.001 ms | 0.7785 ms | 2.2463 ms |   4.804 ms |     1.64x faster |   0.58x |       400 B | 94,707.080x less |
| Fennecs                                     | 100000      | 32        |                                     80.629 ms | 0.9835 ms | 0.7678 ms |  80.629 ms |     9.53x slower |   2.25x | 113600400 B |      2.999x more |
| FlecsNET                                    | 100000      | 32        |                                    190.219 ms | 3.5987 ms | 3.5344 ms | 188.458 ms |    22.49x slower |   5.31x |       400 B | 94,707.080x less |
| Friflo                                      | 100000      | 32        |                                      4.713 ms | 0.0442 ms | 0.0786 ms |   4.704 ms |     1.89x faster |   0.42x |       400 B | 94,707.080x less |
| LeoEcs                                      | 100000      | 32        |                                     15.755 ms | 0.2901 ms | 0.2849 ms |  15.808 ms |     1.86x slower |   0.44x |   8800400 B |      4.305x less |
| LeoEcsLite                                  | 100000      | 32        |                                      6.991 ms | 0.1865 ms | 0.4946 ms |   6.811 ms |     1.28x faster |   0.29x |       400 B | 94,707.080x less |
| Morpeh                                      | 100000      | 32        |                                      4.287 ms | 0.0846 ms | 0.1101 ms |   4.256 ms |     2.08x faster |   0.46x |       400 B | 94,707.080x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | 32        | <span style="color: lightgreen;">**2.404 ms** | 0.2598 ms | 0.7326 ms |   1.991 ms | **4.01x faster** |   1.38x |       400 B | 94,707.080x less |
| TinyEcs                                     | 100000      | 32        |                                     17.252 ms | 0.3432 ms | 0.7746 ms |  17.211 ms |     2.04x slower |   0.49x |   8341648 B |      4.541x less |
| Xeno                                        | 100000      | 32        |                                      3.103 ms | 0.0815 ms | 0.2176 ms |   3.136 ms |     2.88x faster |   0.67x |       400 B | 94,707.080x less |

# CreateEntityWith3Components

| Context                                     | EntityCount |                                          Mean |     Error |    StdDev |            Ratio | RatioSD |   Allocated |      Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|----------:|----------:|-----------------:|--------:|------------:|-----------------:|
| Arch                                        | 100000      |                                      3.107 ms | 0.0581 ms | 0.0670 ms |         baseline |         |      1072 B |                  |
| DefaultECS                                  | 100000      |                                      5.759 ms | 0.0975 ms | 0.0864 ms |     1.85x slower |   0.05x |       400 B |       2.68x less |
| DragonECS                                   | 100000      |                                      3.183 ms | 0.0436 ms | 0.0387 ms |     1.03x slower |   0.02x |       400 B |       2.68x less |
| Fennecs                                     | 100000      |                                    136.069 ms | 2.6476 ms | 3.6240 ms |    43.82x slower |   1.45x | 184800400 B | 172,388.43x more |
| FlecsNET                                    | 100000      |                                    241.771 ms | 3.5409 ms | 5.7179 ms |    77.86x slower |   2.40x |       400 B |       2.68x less |
| Friflo                                      | 100000      |                                      8.304 ms | 0.1450 ms | 0.1357 ms |     2.67x slower |   0.07x |       400 B |       2.68x less |
| LeoEcs                                      | 100000      |                                      8.867 ms | 0.1519 ms | 0.1421 ms |     2.86x slower |   0.07x |       112 B |       9.57x less |
| LeoEcsLite                                  | 100000      |                                      6.708 ms | 0.0621 ms | 0.0519 ms |     2.16x slower |   0.05x |       400 B |       2.68x less |
| Morpeh                                      | 100000      |                                      4.455 ms | 0.0860 ms | 0.1388 ms |     1.43x slower |   0.05x |       400 B |       2.68x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**1.167 ms** | 0.0103 ms | 0.0091 ms | **2.66x faster** |   0.06x |       400 B |       2.68x less |
| TinyEcs                                     | 100000      |                                     21.173 ms | 0.4218 ms | 0.9081 ms |     6.82x slower |   0.32x |  10889200 B |  10,157.84x more |
| Xeno                                        | 100000      |                                      5.301 ms | 0.0930 ms | 0.0726 ms |     1.71x slower |   0.04x |   1045160 B |     974.96x more |

# CreateEntityWith3RandomComponents

| Context                                     | EntityCount | ChunkSize |                                          Mean |     Error |    StdDev |     Median |            Ratio | RatioSD |    Allocated |       Alloc Ratio |
|---------------------------------------------|-------------|-----------|----------------------------------------------:|----------:|----------:|-----------:|-----------------:|--------:|-------------:|------------------:|
| Arch                                        | 100000      | 1         |                                     12.422 ms | 1.2945 ms | 3.7350 ms |  11.515 ms |         baseline |         |   47701336 B |                   |
| DefaultECS                                  | 100000      | 1         |                                      9.907 ms | 0.1963 ms | 0.1639 ms |   9.851 ms |     1.25x faster |   0.38x |  204527184 B |       4.288x more |
| DragonECS                                   | 100000      | 1         |                                      7.698 ms | 0.1422 ms | 0.3031 ms |   7.644 ms |     1.62x faster |   0.49x |        400 B | 119,253.340x less |
| Fennecs                                     | 100000      | 1         |                                    141.346 ms | 1.3644 ms | 1.1393 ms | 140.905 ms |    12.30x slower |   3.22x |  184800400 B |       3.874x more |
| FlecsNET                                    | 100000      | 1         |                                    312.960 ms | 6.0805 ms | 7.4674 ms | 310.873 ms |    27.24x slower |   7.16x |        400 B | 119,253.340x less |
| Friflo                                      | 100000      | 1         |                                     10.821 ms | 0.1996 ms | 0.1867 ms |  10.757 ms |     1.15x faster |   0.34x |        400 B | 119,253.340x less |
| LeoEcs                                      | 100000      | 1         |                                     29.435 ms | 0.5817 ms | 1.1750 ms |  29.214 ms |     2.56x slower |   0.68x |    8800400 B |       5.420x less |
| LeoEcsLite                                  | 100000      | 1         |                                     14.470 ms | 0.2752 ms | 0.3380 ms |  14.402 ms |     1.26x slower |   0.33x |        400 B | 119,253.340x less |
| Morpeh                                      | 100000      | 1         |                                      9.692 ms | 0.2498 ms | 0.7004 ms |   9.586 ms |     1.29x faster |   0.40x |        400 B | 119,253.340x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | 1         | <span style="color: lightgreen;">**4.708 ms** | 0.1391 ms | 0.3900 ms |   4.674 ms | **2.66x faster** |   0.83x |        400 B | 119,253.340x less |
| TinyEcs                                     | 100000      | 1         |                                     33.097 ms | 0.6168 ms | 1.0964 ms |  32.680 ms |     2.88x slower |   0.76x | 1352984536 B |      28.364x more |
| Xeno                                        | 100000      | 1         |                                      6.710 ms | 0.1306 ms | 0.3180 ms |   6.792 ms |     1.86x faster |   0.56x |        400 B | 119,253.340x less |
|                                             |             |           |                                               |           |           |            |                  |         |              |                   |
| Arch                                        | 100000      | 4         |                                     11.329 ms | 1.4060 ms | 4.1235 ms |   9.964 ms |         baseline |         |   46936456 B |                   |
| DefaultECS                                  | 100000      | 4         |                                      8.473 ms | 0.1663 ms | 0.1980 ms |   8.427 ms |     1.34x faster |   0.49x |    3200400 B |      14.666x less |
| DragonECS                                   | 100000      | 4         |                                      5.777 ms | 0.1354 ms | 0.3566 ms |   5.668 ms |     1.97x faster |   0.72x |        400 B | 117,341.140x less |
| Fennecs                                     | 100000      | 4         |                                    136.650 ms | 1.0878 ms | 0.9084 ms | 136.236 ms |    13.52x slower |   4.20x |  184800400 B |       3.937x more |
| FlecsNET                                    | 100000      | 4         |                                    273.751 ms | 4.8557 ms | 4.7689 ms | 271.550 ms |    27.08x slower |   8.43x |        400 B | 117,341.140x less |
| Friflo                                      | 100000      | 4         |                                      8.768 ms | 0.1668 ms | 0.1393 ms |   8.829 ms |     1.29x faster |   0.47x |        400 B | 117,341.140x less |
| LeoEcs                                      | 100000      | 4         |                                     25.680 ms | 0.5034 ms | 1.0283 ms |  25.532 ms |     2.54x slower |   0.80x |    8800400 B |       5.333x less |
| LeoEcsLite                                  | 100000      | 4         |                                     13.293 ms | 0.3256 ms | 0.8858 ms |  12.978 ms |     1.31x slower |   0.42x |        400 B | 117,341.140x less |
| Morpeh                                      | 100000      | 4         |                                      6.772 ms | 0.1664 ms | 0.4610 ms |   6.681 ms |     1.68x faster |   0.62x |        400 B | 117,341.140x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | 4         | <span style="color: lightgreen;">**3.471 ms** | 0.1665 ms | 0.4695 ms |   3.303 ms | **3.32x faster** |   1.28x |        400 B | 117,341.140x less |
| TinyEcs                                     | 100000      | 4         |                                     24.749 ms | 0.4501 ms | 0.9688 ms |  24.467 ms |     2.45x slower |   0.77x |   10823616 B |       4.336x less |
| Xeno                                        | 100000      | 4         |                                      4.806 ms | 0.0987 ms | 0.2668 ms |   4.883 ms |     2.37x faster |   0.87x |        400 B | 117,341.140x less |
|                                             |             |           |                                               |           |           |            |                  |         |              |                   |
| Arch                                        | 100000      | 32        |                                     10.322 ms | 1.0482 ms | 3.0410 ms |   9.726 ms |         baseline |         |   47303848 B |                   |
| DefaultECS                                  | 100000      | 32        |                                      7.929 ms | 0.1575 ms | 0.2543 ms |   7.864 ms |     1.30x faster |   0.38x |    3200400 B |      14.781x less |
| DragonECS                                   | 100000      | 32        |                                      5.323 ms | 0.1309 ms | 0.3424 ms |   5.301 ms |     1.95x faster |   0.58x |        400 B | 118,259.620x less |
| Fennecs                                     | 100000      | 32        |                                    138.701 ms | 2.4187 ms | 2.7854 ms | 138.930 ms |    14.56x slower |   3.99x |  184800400 B |       3.907x more |
| FlecsNET                                    | 100000      | 32        |                                    259.107 ms | 5.0785 ms | 4.5020 ms | 257.524 ms |    27.20x slower |   7.45x |        400 B | 118,259.620x less |
| Friflo                                      | 100000      | 32        |                                      7.506 ms | 0.1322 ms | 0.1032 ms |   7.511 ms |     1.38x faster |   0.40x |        400 B | 118,259.620x less |
| LeoEcs                                      | 100000      | 32        |                                     22.307 ms | 0.4437 ms | 0.9833 ms |  21.937 ms |     2.34x slower |   0.65x |   92126192 B |       1.948x more |
| LeoEcsLite                                  | 100000      | 32        |                                     11.763 ms | 0.2342 ms | 0.5335 ms |  11.556 ms |     1.23x slower |   0.34x |        400 B | 118,259.620x less |
| Morpeh                                      | 100000      | 32        |                                      5.705 ms | 0.1124 ms | 0.2296 ms |   5.757 ms |     1.81x faster |   0.54x |        400 B | 118,259.620x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | 32        | <span style="color: lightgreen;">**3.588 ms** | 0.2023 ms | 0.5605 ms |   3.436 ms | **2.94x faster** |   0.96x |        400 B | 118,259.620x less |
| TinyEcs                                     | 100000      | 32        |                                     21.457 ms | 0.3327 ms | 0.5180 ms |  21.328 ms |     2.25x slower |   0.62x |   10823616 B |       4.370x less |
| Xeno                                        | 100000      | 32        |                                      3.909 ms | 0.1055 ms | 0.2853 ms |   3.865 ms |     2.65x faster |   0.80x |        400 B | 118,259.620x less |

# CreateEntityWith4Components

| Context                                     | EntityCount |                                          Mean |     Error |    StdDev |     Median |            Ratio | RatioSD |   Allocated |      Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|----------:|----------:|-----------:|-----------------:|--------:|------------:|-----------------:|
| Arch                                        | 100000      |                                      3.324 ms | 0.0505 ms | 0.0422 ms |   3.312 ms |         baseline |         |      1072 B |                  |
| DefaultECS                                  | 100000      |                                      7.101 ms | 0.0742 ms | 0.0658 ms |   7.100 ms |     2.14x slower |   0.03x |       400 B |       2.68x less |
| DragonECS                                   | 100000      |                                      4.003 ms | 0.1076 ms | 0.3122 ms |   3.898 ms |     1.20x slower |   0.09x |       400 B |       2.68x less |
| Fennecs                                     | 100000      |                                    196.846 ms | 1.2350 ms | 1.0948 ms | 196.696 ms |    59.23x slower |   0.79x | 268800112 B | 250,746.37x more |
| FlecsNET                                    | 100000      |                                    319.231 ms | 3.6476 ms | 3.4119 ms | 320.355 ms |    96.06x slower |   1.53x |       400 B |       2.68x less |
| Friflo                                      | 100000      |                                     10.342 ms | 0.0490 ms | 0.0435 ms |  10.345 ms |     3.11x slower |   0.04x |       400 B |       2.68x less |
| LeoEcs                                      | 100000      |                                     12.032 ms | 0.1366 ms | 0.1141 ms |  12.026 ms |     3.62x slower |   0.06x |       400 B |       2.68x less |
| LeoEcsLite                                  | 100000      |                                      9.462 ms | 0.1857 ms | 0.3301 ms |   9.303 ms |     2.85x slower |   0.10x |       400 B |       2.68x less |
| Morpeh                                      | 100000      |                                      5.702 ms | 0.1124 ms | 0.1051 ms |   5.699 ms |     1.72x slower |   0.04x |       400 B |       2.68x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**1.693 ms** | 0.0896 ms | 0.2512 ms |   1.590 ms | **2.00x faster** |   0.25x |       400 B |       2.68x less |
| TinyEcs                                     | 100000      |                                     32.393 ms | 0.9152 ms | 2.4743 ms |  31.756 ms |     9.75x slower |   0.75x |  13289200 B |  12,396.64x more |
| Xeno                                        | 100000      |                                      4.201 ms | 0.1304 ms | 0.3479 ms |   4.111 ms |     1.26x slower |   0.11x |   1045160 B |     974.96x more |

# DeleteEntity

| Context                                     | EntityCount |                                           Mean |     Error |      StdDev |     Median |            Ratio | RatioSD | Allocated | Alloc Ratio |
|---------------------------------------------|-------------|-----------------------------------------------:|----------:|------------:|-----------:|-----------------:|--------:|----------:|------------:|
| Arch                                        | 100000      |                                     1,130.2 ns | 133.54 ns |   385.29 ns | 1,100.0 ns |         baseline |         |    1072 B |             |
| DefaultECS                                  | 100000      |                                     2,269.9 ns | 210.21 ns |   596.33 ns | 2,100.0 ns |     2.30x slower |   1.15x |     400 B |  2.68x less |
| DragonECS                                   | 100000      |                                       334.9 ns |  30.76 ns |    83.69 ns |   300.0 ns |     3.61x faster |   1.60x |     400 B |  2.68x less |
| Fennecs                                     | 100000      |                                     1,284.0 ns | 197.98 ns |   583.74 ns | 1,100.0 ns |     1.30x slower |   0.84x |     112 B |  9.57x less |
| FlecsNET                                    | 100000      |                                       795.8 ns |  48.21 ns |   139.11 ns |   800.0 ns |     1.46x faster |   0.55x |     400 B |  2.68x less |
| Friflo                                      | 100000      |                                     2,629.6 ns | 423.18 ns | 1,114.84 ns | 2,300.0 ns |     2.67x slower |   1.64x |     136 B |  7.88x less |
| LeoEcs                                      | 100000      |                                     2,708.5 ns | 237.42 ns |   677.39 ns | 2,650.0 ns |     2.75x slower |   1.36x |      64 B | 16.75x less |
| LeoEcsLite                                  | 100000      |                                       258.0 ns |  25.06 ns |    69.02 ns |   200.0 ns |     4.70x faster |   2.09x |     400 B |  2.68x less |
| Morpeh                                      | 100000      |                                     2,850.0 ns | 291.63 ns |   822.55 ns | 2,600.0 ns |     2.89x slower |   1.50x |     400 B |  2.68x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**175.3 ns]** |  19.90 ns |    56.45 ns |   200.0 ns | **7.29x faster** |   3.80x |     400 B |  2.68x less |
| TinyEcs                                     | 100000      |                                     3,349.4 ns | 862.82 ns | 2,361.95 ns | 2,500.0 ns |     3.40x slower |   2.94x |      88 B | 12.18x less |
| Xeno                                        | 100000      |                                       837.6 ns |  89.86 ns |   249.01 ns |   750.0 ns |     1.45x faster |   0.61x |      88 B | 12.18x less |

# FourRemoveOneComponent

| Context                                     | EntityCount |                                          Mean |     Error |    StdDev |      Median |             Ratio | RatioSD |  Allocated |       Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|----------:|----------:|------------:|------------------:|--------:|-----------:|------------------:|
| Arch                                        | 100000      |                                   39,794.5 μs | 456.30 μs | 356.25 μs | 39,786.9 μs |          baseline |         | 25602304 B |                   |
| DefaultECS                                  | 100000      |                                    1,100.0 μs |  22.02 μs |  42.94 μs |  1,087.9 μs |     36.23x faster |   1.37x |      400 B |  64,005.760x less |
| DragonECS                                   | 100000      |                                    1,008.6 μs |  23.65 μs |  63.52 μs |    989.0 μs |     39.60x faster |   2.29x |      400 B |  64,005.760x less |
| Fennecs                                     | 100000      |                                   62,572.3 μs | 864.21 μs | 674.71 μs | 62,527.1 μs |      1.57x slower |   0.02x | 65600400 B |       2.562x more |
| FlecsNET                                    | 100000      |                                   72,955.0 μs | 905.18 μs | 846.71 μs | 72,628.8 μs |      1.83x slower |   0.03x |      400 B |  64,005.760x less |
| Friflo                                      | 100000      |                                    2,605.4 μs |  39.27 μs |  36.73 μs |  2,590.1 μs |     15.28x faster |   0.24x |      400 B |  64,005.760x less |
| LeoEcs                                      | 100000      |                                    7,991.6 μs |  77.96 μs |  69.11 μs |  7,989.9 μs |      4.98x faster |   0.06x |      112 B | 228,592.000x less |
| LeoEcsLite                                  | 100000      |                                    3,494.8 μs |  39.87 μs |  33.30 μs |  3,486.6 μs |     11.39x faster |   0.14x |      400 B |  64,005.760x less |
| Morpeh                                      | 100000      |                                    3,015.9 μs |  59.97 μs | 163.15 μs |  3,014.7 μs |     13.23x faster |   0.72x |      400 B |  64,005.760x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**443.6 μs** |   8.86 μs |  19.62 μs |    434.6 μs | **89.88x faster** |   3.85x |      400 B |  64,005.760x less |
| TinyEcs                                     | 100000      |                                      722.9 μs |  11.77 μs |  10.43 μs |    722.0 μs |     55.06x faster |   0.90x |      400 B |  64,005.760x less |
| Xeno                                        | 100000      |                                    2,529.0 μs |  20.22 μs |  16.88 μs |  2,522.6 μs |     15.74x faster |   0.17x |      400 B |  64,005.760x less |

# FourRemoveThreeComponents

| Context                                     | EntityCount |                                          Mean |     Error |    StdDev |             Ratio | RatioSD |   Allocated |      Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|----------:|----------:|------------------:|--------:|------------:|-----------------:|
| Arch                                        | 100000      |                                     41.217 ms | 0.1730 ms | 0.1534 ms |          baseline |         |  25602304 B |                  |
| DefaultECS                                  | 100000      |                                      3.025 ms | 0.0444 ms | 0.0436 ms |     13.63x faster |   0.20x |       400 B | 64,005.760x less |
| DragonECS                                   | 100000      |                                      2.451 ms | 0.0326 ms | 0.0527 ms |     16.83x faster |   0.35x |       400 B | 64,005.760x less |
| Fennecs                                     | 100000      |                                    151.130 ms | 1.2281 ms | 1.0255 ms |      3.67x slower |   0.03x | 162400400 B |      6.343x more |
| FlecsNET                                    | 100000      |                                    164.282 ms | 1.9170 ms | 1.6008 ms |      3.99x slower |   0.04x |       400 B | 64,005.760x less |
| Friflo                                      | 100000      |                                      5.914 ms | 0.1050 ms | 0.0876 ms |      6.97x faster |   0.10x |       400 B | 64,005.760x less |
| LeoEcs                                      | 100000      |                                     15.002 ms | 0.1684 ms | 0.1406 ms |      2.75x faster |   0.03x |       400 B | 64,005.760x less |
| LeoEcsLite                                  | 100000      |                                      8.760 ms | 0.1224 ms | 0.1145 ms |      4.71x faster |   0.06x |       400 B | 64,005.760x less |
| Morpeh                                      | 100000      |                                      2.793 ms | 0.0492 ms | 0.0483 ms |     14.76x faster |   0.25x |       400 B | 64,005.760x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**1.477 ms** | 0.0088 ms | 0.0078 ms | **27.90x faster** |   0.17x |       400 B | 64,005.760x less |
| TinyEcs                                     | 100000      |                                      2.334 ms | 0.0418 ms | 0.0349 ms |     17.66x faster |   0.26x |       400 B | 64,005.760x less |
| Xeno                                        | 100000      |                                      3.509 ms | 0.0492 ms | 0.0436 ms |     11.75x faster |   0.15x |       400 B | 64,005.760x less |

# FourRemoveTwoComponents

| Context                                     | EntityCount |                                          Mean |       Error |      StdDev |       Median |             Ratio | RatioSD |   Allocated |      Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|------------:|------------:|-------------:|------------------:|--------:|------------:|-----------------:|
| Arch                                        | 100000      |                                   40,428.8 μs |   306.36 μs |   271.58 μs |  40,499.8 μs |          baseline |         |  25602304 B |                  |
| DefaultECS                                  | 100000      |                                    2,027.3 μs |    14.73 μs |    12.30 μs |   2,030.4 μs |     19.94x faster |   0.17x |       400 B | 64,005.760x less |
| DragonECS                                   | 100000      |                                    1,699.9 μs |    28.43 μs |    70.28 μs |   1,678.6 μs |     23.82x faster |   0.94x |       400 B | 64,005.760x less |
| Fennecs                                     | 100000      |                                  107,672.5 μs | 2,121.45 μs | 1,984.41 μs | 107,236.1 μs |      2.66x slower |   0.05x | 118400064 B |      4.625x more |
| FlecsNET                                    | 100000      |                                  120,569.8 μs | 2,124.96 μs | 1,774.44 μs | 119,945.9 μs |      2.98x slower |   0.05x |       400 B | 64,005.760x less |
| Friflo                                      | 100000      |                                    4,304.9 μs |    67.14 μs |    62.81 μs |   4,272.4 μs |      9.39x faster |   0.15x |       400 B | 64,005.760x less |
| LeoEcs                                      | 100000      |                                   11,092.9 μs |    75.39 μs |    58.86 μs |  11,120.0 μs |      3.64x faster |   0.03x |       400 B | 64,005.760x less |
| LeoEcsLite                                  | 100000      |                                    6,012.2 μs |    50.01 μs |    46.78 μs |   6,013.8 μs |      6.72x faster |   0.07x |       400 B | 64,005.760x less |
| Morpeh                                      | 100000      |                                    3,337.9 μs |    66.54 μs |   111.17 μs |   3,316.2 μs |     12.12x faster |   0.40x |       400 B | 64,005.760x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**950.0 μs** |    11.53 μs |    10.22 μs |     947.2 μs | **42.56x faster** |   0.51x |       400 B | 64,005.760x less |
| TinyEcs                                     | 100000      |                                    1,275.2 μs |    67.06 μs |   196.69 μs |   1,172.2 μs |     32.37x faster |   4.36x |       400 B | 64,005.760x less |
| Xeno                                        | 100000      |                                    3,270.3 μs |    61.75 μs |   103.18 μs |   3,270.7 μs |     12.37x faster |   0.39x |   1045224 B |     24.495x less |

# OneAddOneComponent

| Context                                     | EntityCount |                                          Mean |       Error |      StdDev |             Ratio | RatioSD |  Allocated |     Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|------------:|------------:|------------------:|--------:|-----------:|----------------:|
| Arch                                        | 100000      |                                    6,403.7 μs |    58.20 μs |    45.44 μs |          baseline |         |     1072 B |                 |
| DefaultECS                                  | 100000      |                                    1,336.1 μs |    21.72 μs |    41.84 μs |      4.80x faster |   0.15x |      400 B |      2.68x less |
| DragonECS                                   | 100000      |                                      859.3 μs |    17.15 μs |    47.52 μs |      7.47x faster |   0.40x |      400 B |      2.68x less |
| Fennecs                                     | 100000      |                                   44,503.9 μs |   577.75 μs |   540.43 μs |      6.95x slower |   0.09x | 64000400 B | 59,701.87x more |
| FlecsNET                                    | 100000      |                                   89,494.0 μs | 1,075.31 μs | 1,005.85 μs |     13.98x slower |   0.18x |      400 B |      2.68x less |
| Friflo                                      | 100000      |                                    1,993.6 μs |    28.51 μs |    28.00 μs |      3.21x faster |   0.05x |      400 B |      2.68x less |
| LeoEcs                                      | 100000      |                                    3,621.5 μs |    50.80 μs |    47.52 μs |      1.77x faster |   0.03x |      400 B |      2.68x less |
| LeoEcsLite                                  | 100000      |                                    1,556.9 μs |    11.36 μs |     9.49 μs |      4.11x faster |   0.04x |      400 B |      2.68x less |
| Morpeh                                      | 100000      |                                    2,989.8 μs |    59.70 μs |   158.31 μs |      2.15x faster |   0.11x |      400 B |      2.68x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**330.7 μs** |     6.16 μs |     5.15 μs | **19.37x faster** |   0.32x |      400 B |      2.68x less |
| TinyEcs                                     | 100000      |                                    3,738.1 μs |    73.79 μs |    78.95 μs |      1.71x faster |   0.04x |  2400400 B |  2,239.18x more |
| Xeno                                        | 100000      |                                    2,440.8 μs |    39.57 μs |    60.43 μs |      2.63x faster |   0.06x |  1045224 B |    975.02x more |

# OneAddThreeComponents

| Context                                     | EntityCount |                                          Mean |     Error |    StdDev |            Ratio | RatioSD |   Allocated |      Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|----------:|----------:|-----------------:|--------:|------------:|-----------------:|
| Arch                                        | 100000      |                                      7.917 ms | 0.0713 ms | 0.0595 ms |         baseline |         |      1072 B |                  |
| DefaultECS                                  | 100000      |                                      4.166 ms | 0.0380 ms | 0.0356 ms |     1.90x faster |   0.02x |       400 B |       2.68x less |
| DragonECS                                   | 100000      |                                      2.082 ms | 0.0240 ms | 0.0200 ms |     3.80x faster |   0.04x |       400 B |       2.68x less |
| Fennecs                                     | 100000      |                                    168.570 ms | 3.2663 ms | 5.3667 ms |    21.29x slower |   0.69x | 219200400 B | 204,477.99x more |
| FlecsNET                                    | 100000      |                                    269.503 ms | 5.0398 ms | 4.4676 ms |    34.04x slower |   0.60x |       400 B |       2.68x less |
| Friflo                                      | 100000      |                                      6.838 ms | 0.0447 ms | 0.0418 ms |     1.16x faster |   0.01x |       400 B |       2.68x less |
| LeoEcs                                      | 100000      |                                      8.963 ms | 0.0638 ms | 0.0566 ms |     1.13x slower |   0.01x |       400 B |       2.68x less |
| LeoEcsLite                                  | 100000      |                                      6.430 ms | 0.0280 ms | 0.0234 ms |     1.23x faster |   0.01x |       400 B |       2.68x less |
| Morpeh                                      | 100000      |                                      4.311 ms | 0.0618 ms | 0.0516 ms |     1.84x faster |   0.02x |       400 B |       2.68x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**1.075 ms** | 0.0144 ms | 0.0120 ms | **7.37x faster** |   0.10x |       400 B |       2.68x less |
| TinyEcs                                     | 100000      |                                     20.371 ms | 0.3871 ms | 0.3022 ms |     2.57x slower |   0.04x |   7200400 B |   6,716.79x more |
| Xeno                                        | 100000      |                                      3.754 ms | 0.0747 ms | 0.0860 ms |     2.11x faster |   0.05x |   1045232 B |     975.03x more |

# OneAddTwoComponents

| Context                                     | EntityCount |                                          Mean |       Error |      StdDev |       Median |             Ratio | RatioSD |   Allocated |      Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|------------:|------------:|-------------:|------------------:|--------:|------------:|-----------------:|
| Arch                                        | 100000      |                                    7,781.6 μs |   152.40 μs |   135.10 μs |   7,745.6 μs |          baseline |         |      1072 B |                  |
| DefaultECS                                  | 100000      |                                    2,625.2 μs |    50.51 μs |    49.61 μs |   2,631.4 μs |      2.97x faster |   0.07x |       400 B |       2.68x less |
| DragonECS                                   | 100000      |                                    1,325.9 μs |    25.89 μs |    24.22 μs |   1,318.3 μs |      5.87x faster |   0.14x |       400 B |       2.68x less |
| Fennecs                                     | 100000      |                                   99,828.4 μs | 1,963.30 μs | 2,260.94 μs |  99,141.1 μs |     12.83x slower |   0.35x | 135200400 B | 126,119.78x more |
| FlecsNET                                    | 100000      |                                  163,825.4 μs | 2,649.73 μs | 2,478.56 μs | 164,164.8 μs |     21.06x slower |   0.46x |       400 B |       2.68x less |
| Friflo                                      | 100000      |                                    4,278.7 μs |    64.34 μs |    57.04 μs |   4,261.6 μs |      1.82x faster |   0.04x |       400 B |       2.68x less |
| LeoEcs                                      | 100000      |                                    5,891.6 μs |   107.81 μs |   100.85 μs |   5,902.4 μs |      1.32x faster |   0.03x |       400 B |       2.68x less |
| LeoEcsLite                                  | 100000      |                                    3,493.0 μs |    55.36 μs |    46.23 μs |   3,485.2 μs |      2.23x faster |   0.05x |       400 B |       2.68x less |
| Morpeh                                      | 100000      |                                    3,621.0 μs |    83.14 μs |   234.49 μs |   3,588.7 μs |      2.16x faster |   0.14x |       400 B |       2.68x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**733.5 μs** |    22.92 μs |    64.64 μs |     712.0 μs | **10.68x faster** |   0.88x |       400 B |       2.68x less |
| TinyEcs                                     | 100000      |                                    6,969.4 μs |   133.92 μs |   159.42 μs |   6,931.8 μs |      1.12x faster |   0.03x |   4800400 B |   4,477.99x more |
| Xeno                                        | 100000      |                                    3,145.7 μs |    60.90 μs |    74.78 μs |   3,111.2 μs |      2.48x faster |   0.07x |   1045232 B |     975.03x more |

# Remove1ComponentRandomOrder

| Context                                     | EntityCount |                                          Mean |     Error |    StdDev |    Median |             Ratio | RatioSD |  Allocated |       Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|----------:|----------:|----------:|------------------:|--------:|-----------:|------------------:|
| Arch                                        | 100000      |                                     45.851 ms | 1.4942 ms | 4.2873 ms | 45.555 ms |          baseline |         | 43086488 B |                   |
| DefaultECS                                  | 100000      |                                     10.045 ms | 0.5452 ms | 1.5818 ms |  9.709 ms |      4.67x faster |   0.80x |      400 B | 107,716.220x less |
| DragonECS                                   | 100000      |                                      6.198 ms | 0.3028 ms | 0.8640 ms |  5.991 ms |      7.53x faster |   1.17x |      400 B | 107,716.220x less |
| Fennecs                                     | 100000      |                                     42.932 ms | 0.8941 ms | 2.5219 ms | 42.582 ms |      1.07x faster |   0.12x | 36800064 B |       1.171x less |
| FlecsNET                                    | 100000      |                                     93.715 ms | 2.8279 ms | 8.2492 ms | 93.028 ms |      2.06x slower |   0.26x |      400 B | 107,716.220x less |
| Friflo                                      | 100000      |                                      4.801 ms | 0.3389 ms | 0.9613 ms |  4.564 ms |      9.89x faster |   1.98x |      400 B | 107,716.220x less |
| LeoEcs                                      | 100000      |                                     39.002 ms | 1.5033 ms | 4.1905 ms | 38.746 ms |      1.19x faster |   0.17x |      400 B | 107,716.220x less |
| LeoEcsLite                                  | 100000      |                                      6.679 ms | 0.5781 ms | 1.6680 ms |  6.360 ms |      7.27x faster |   1.83x |      400 B | 107,716.220x less |
| Morpeh                                      | 100000      |                                     19.844 ms | 0.7795 ms | 2.2240 ms | 19.174 ms |      2.34x faster |   0.33x |      400 B | 107,716.220x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**2.482 ms** | 0.1624 ms | 0.4555 ms |  2.371 ms | **19.01x faster** |   3.49x |       64 B | 673,226.375x less |
| TinyEcs                                     | 100000      |                                     13.360 ms | 0.9276 ms | 2.6163 ms | 12.694 ms |      3.55x faster |   0.70x |  1575160 B |      27.354x less |
| Xeno                                        | 100000      |                                      8.007 ms | 0.4412 ms | 1.2728 ms |  8.007 ms |      5.87x faster |   1.09x |      400 B | 107,716.220x less |

# Remove1Component

| Context                                     | EntityCount |                                          Mean |     Error |      StdDev |      Median |             Ratio | RatioSD |  Allocated |       Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|----------:|------------:|------------:|------------------:|--------:|-----------:|------------------:|
| Arch                                        | 100000      |                                   36,450.4 μs | 879.81 μs | 2,566.46 μs | 36,877.9 μs |          baseline |         | 43085872 B |                   |
| DefaultECS                                  | 100000      |                                    1,045.7 μs |  20.22 μs |    27.68 μs |  1,034.0 μs |     34.88x faster |   2.60x |      400 B | 107,714.680x less |
| DragonECS                                   | 100000      |                                    1,139.0 μs |  37.86 μs |    99.06 μs |  1,101.3 μs |     32.21x faster |   3.30x |      400 B | 107,714.680x less |
| Fennecs                                     | 100000      |                                   31,040.5 μs | 328.92 μs |   274.67 μs | 30,992.5 μs |      1.17x faster |   0.08x | 36800400 B |       1.171x less |
| FlecsNET                                    | 100000      |                                   53,376.1 μs | 382.24 μs |   319.19 μs | 53,317.4 μs |      1.47x slower |   0.11x |      400 B | 107,714.680x less |
| Friflo                                      | 100000      |                                    1,369.3 μs |  26.51 μs |    38.02 μs |  1,356.8 μs |     26.64x faster |   2.00x |      400 B | 107,714.680x less |
| LeoEcs                                      | 100000      |                                    3,804.2 μs |  27.76 μs |    67.56 μs |  3,796.2 μs |      9.58x faster |   0.69x |      400 B | 107,714.680x less |
| LeoEcsLite                                  | 100000      |                                    1,821.4 μs |  18.20 μs |    36.35 μs |  1,811.0 μs |     20.02x faster |   1.45x |      400 B | 107,714.680x less |
| Morpeh                                      | 100000      |                                    2,767.4 μs |  91.08 μs |   259.85 μs |  2,728.8 μs |     13.28x faster |   1.50x |      400 B | 107,714.680x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**736.6 μs** |  13.02 μs |    12.78 μs |    735.0 μs | **49.50x faster** |   3.57x |      400 B | 107,714.680x less |
| TinyEcs                                     | 100000      |                                    3,774.6 μs |  74.23 μs |   131.94 μs |  3,774.0 μs |      9.67x faster |   0.75x |  1575160 B |      27.353x less |
| Xeno                                        | 100000      |                                    2,391.3 μs |  19.61 μs |    26.18 μs |  2,383.4 μs |     15.24x faster |   1.08x |      400 B | 107,714.680x less |

# Remove2ComponentsRandomOrder

| Context                                     | EntityCount |                                          Mean |     Error |    StdDev |     Median |             Ratio | RatioSD |  Allocated |       Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|----------:|----------:|-----------:|------------------:|--------:|-----------:|------------------:|
| Arch                                        | 100000      |                                     50.450 ms | 1.0474 ms | 2.9713 ms |  50.668 ms |          baseline |         | 43885720 B |                   |
| DefaultECS                                  | 100000      |                                     17.653 ms | 0.5034 ms | 1.4525 ms |  17.406 ms |      2.88x faster |   0.28x |      400 B | 109,714.300x less |
| DragonECS                                   | 100000      |                                     10.661 ms | 0.2113 ms | 0.4071 ms |  10.592 ms |      4.74x faster |   0.33x |      400 B | 109,714.300x less |
| Fennecs                                     | 100000      |                                     86.662 ms | 1.4868 ms | 1.4602 ms |  86.619 ms |      1.72x slower |   0.11x | 75200400 B |       1.714x more |
| FlecsNET                                    | 100000      |                                    137.380 ms | 2.6715 ms | 3.4736 ms | 137.243 ms |      2.73x slower |   0.18x |      400 B | 109,714.300x less |
| Friflo                                      | 100000      |                                      9.785 ms | 0.9561 ms | 2.8040 ms |   9.019 ms |      5.55x faster |   1.47x |      400 B | 109,714.300x less |
| LeoEcs                                      | 100000      |                                     39.488 ms | 1.0214 ms | 2.9471 ms |  38.858 ms |      1.28x faster |   0.12x |      400 B | 109,714.300x less |
| LeoEcsLite                                  | 100000      |                                     11.554 ms | 0.6312 ms | 1.8009 ms |  11.254 ms |      4.47x faster |   0.71x |      400 B | 109,714.300x less |
| Morpeh                                      | 100000      |                                     29.248 ms | 1.0029 ms | 2.8774 ms |  28.288 ms |      1.74x faster |   0.19x |      400 B | 109,714.300x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**5.223 ms** | 0.6286 ms | 1.7832 ms |   4.641 ms | **10.62x faster** |   3.02x |      400 B | 109,714.300x less |
| TinyEcs                                     | 100000      |                                     21.761 ms | 1.7053 ms | 4.9745 ms |  20.619 ms |      2.43x faster |   0.50x |  1575160 B |      27.861x less |
| Xeno                                        | 100000      |                                     11.668 ms | 0.6239 ms | 1.8101 ms |  11.135 ms |      4.42x faster |   0.69x |       64 B | 685,714.375x less |

# Remove2Components

| Context                                     | EntityCount |                                          Mean |     Error |    StdDev |             Ratio | RatioSD |  Allocated |       Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|----------:|----------:|------------------:|--------:|-----------:|------------------:|
| Arch                                        | 100000      |                                     40.892 ms | 1.3082 ms | 3.7953 ms |          baseline |         | 44605712 B |                   |
| DefaultECS                                  | 100000      |                                      2.016 ms | 0.0395 ms | 0.0308 ms |     20.29x faster |   1.90x |      400 B | 111,514.280x less |
| DragonECS                                   | 100000      |                                      6.719 ms | 0.1024 ms | 0.0908 ms |      6.09x faster |   0.57x |      400 B | 111,514.280x less |
| Fennecs                                     | 100000      |                                     70.201 ms | 1.4007 ms | 3.5651 ms |      1.73x slower |   0.18x | 75200400 B |       1.686x more |
| FlecsNET                                    | 100000      |                                    104.584 ms | 1.9587 ms | 1.7363 ms |      2.58x slower |   0.24x |      400 B | 111,514.280x less |
| Friflo                                      | 100000      |                                      2.943 ms | 0.0449 ms | 0.0375 ms |     13.90x faster |   1.29x |      400 B | 111,514.280x less |
| LeoEcs                                      | 100000      |                                      6.799 ms | 0.0378 ms | 0.0316 ms |      6.01x faster |   0.56x |      400 B | 111,514.280x less |
| LeoEcsLite                                  | 100000      |                                      3.441 ms | 0.0511 ms | 0.0453 ms |     11.89x faster |   1.11x |      400 B | 111,514.280x less |
| Morpeh                                      | 100000      |                                      3.292 ms | 0.0704 ms | 0.1997 ms |     12.47x faster |   1.37x |      400 B | 111,514.280x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**1.203 ms** | 0.0075 ms | 0.0063 ms | **33.98x faster** |   3.14x |      400 B | 111,514.280x less |
| TinyEcs                                     | 100000      |                                      7.422 ms | 0.1440 ms | 0.1971 ms |      5.51x faster |   0.53x |  1575160 B |      28.318x less |
| Xeno                                        | 100000      |                                      2.903 ms | 0.0535 ms | 0.0657 ms |     14.09x faster |   1.34x |      400 B | 111,514.280x less |

# Remove3ComponentsRandomOrder

| Context                                     | EntityCount |                                          Mean |     Error |   StdDev |     Median |            Ratio | RatioSD |   Allocated |      Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|----------:|---------:|-----------:|-----------------:|--------:|------------:|-----------------:|
| Arch                                        | 100000      |                                     52.696 ms | 1.0497 ms | 1.997 ms |  52.730 ms |         baseline |         |  36831272 B |                  |
| DefaultECS                                  | 100000      |                                     24.633 ms | 0.6310 ms | 1.780 ms |  23.862 ms |     2.15x faster |   0.16x |       400 B | 92,078.180x less |
| DragonECS                                   | 100000      |                                     20.043 ms | 1.2675 ms | 3.637 ms |  20.165 ms |     2.72x faster |   0.50x |       400 B | 92,078.180x less |
| Fennecs                                     | 100000      |                                    142.722 ms | 2.3351 ms | 2.596 ms | 142.219 ms |     2.71x slower |   0.11x | 128000400 B |      3.475x more |
| FlecsNET                                    | 100000      |                                    194.726 ms | 3.4509 ms | 6.396 ms | 194.064 ms |     3.70x slower |   0.18x |       400 B | 92,078.180x less |
| Friflo                                      | 100000      |                                     13.810 ms | 1.3862 ms | 4.066 ms |  12.350 ms |     4.13x faster |   1.11x |       400 B | 92,078.180x less |
| LeoEcs                                      | 100000      |                                     53.929 ms | 1.7937 ms | 5.204 ms |  53.304 ms |     1.02x slower |   0.11x |       400 B | 92,078.180x less |
| LeoEcsLite                                  | 100000      |                                     16.397 ms | 0.7372 ms | 2.103 ms |  16.216 ms |     3.26x faster |   0.42x |       400 B | 92,078.180x less |
| Morpeh                                      | 100000      |                                     29.525 ms | 0.5878 ms | 1.160 ms |  29.474 ms |     1.79x faster |   0.10x |       400 B | 92,078.180x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**8.960 ms** | 0.7088 ms | 2.011 ms |   8.732 ms | **6.16x faster** |   1.32x |       400 B | 92,078.180x less |
| TinyEcs                                     | 100000      |                                     25.436 ms | 1.0348 ms | 2.969 ms |  24.379 ms |     2.10x faster |   0.24x |   1640744 B |     22.448x less |
| Xeno                                        | 100000      |                                     21.983 ms | 1.2994 ms | 3.770 ms |  21.749 ms |     2.47x faster |   0.42x |       400 B | 92,078.180x less |

# Remove3Components

| Context                                     | EntityCount |                                          Mean |     Error |    StdDev |     Median |             Ratio | RatioSD |   Allocated |      Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|----------:|----------:|-----------:|------------------:|--------:|------------:|-----------------:|
| Arch                                        | 100000      |                                     42.451 ms | 0.7955 ms | 1.6606 ms |  42.794 ms |          baseline |         |  38973776 B |                  |
| DefaultECS                                  | 100000      |                                      3.088 ms | 0.0600 ms | 0.0691 ms |   3.061 ms |     13.75x faster |   0.61x |       400 B | 97,434.440x less |
| DragonECS                                   | 100000      |                                      2.762 ms | 0.0552 ms | 0.0981 ms |   2.725 ms |     15.39x faster |   0.79x |       400 B | 97,434.440x less |
| Fennecs                                     | 100000      |                                    118.240 ms | 2.2181 ms | 4.1114 ms | 117.502 ms |      2.79x slower |   0.15x | 128000400 B |      3.284x more |
| FlecsNET                                    | 100000      |                                    141.532 ms | 1.5414 ms | 1.3664 ms | 141.189 ms |      3.34x slower |   0.14x |       400 B | 97,434.440x less |
| Friflo                                      | 100000      |                                      4.864 ms | 0.0949 ms | 0.1585 ms |   4.822 ms |      8.74x faster |   0.43x |       400 B | 97,434.440x less |
| LeoEcs                                      | 100000      |                                      9.646 ms | 0.1894 ms | 0.2105 ms |   9.564 ms |      4.40x faster |   0.19x |       400 B | 97,434.440x less |
| LeoEcsLite                                  | 100000      |                                      5.704 ms | 0.1065 ms | 0.2651 ms |   5.580 ms |      7.46x faster |   0.43x |       400 B | 97,434.440x less |
| Morpeh                                      | 100000      |                                      2.871 ms | 0.0571 ms | 0.1422 ms |   2.846 ms |     14.82x faster |   0.91x |       400 B | 97,434.440x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**1.812 ms** | 0.0244 ms | 0.0228 ms |   1.813 ms | **23.43x faster** |   0.95x |       400 B | 97,434.440x less |
| TinyEcs                                     | 100000      |                                     12.900 ms | 0.2509 ms | 0.2224 ms |  12.987 ms |      3.29x faster |   0.14x |   1575160 B |     24.743x less |
| Xeno                                        | 100000      |                                      3.525 ms | 0.0629 ms | 0.1270 ms |   3.499 ms |     12.06x faster |   0.63x |       400 B | 97,434.440x less |

# Remove4ComponentsRandomOrder

| Context                                     | EntityCount |                                          Mean |    Error |   StdDev |    Median |            Ratio | RatioSD |   Allocated |       Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|---------:|---------:|----------:|-----------------:|--------:|------------:|------------------:|
| Arch                                        | 100000      |                                      59.47 ms | 1.572 ms | 4.584 ms |  59.79 ms |         baseline |         |  45593408 B |                   |
| DefaultECS                                  | 100000      |                                      33.88 ms | 0.895 ms | 2.538 ms |  32.78 ms |     1.76x faster |   0.18x |       400 B | 113,983.520x less |
| DragonECS                                   | 100000      |                                      22.60 ms | 0.671 ms | 1.945 ms |  22.44 ms |     2.65x faster |   0.30x |       400 B | 113,983.520x less |
| Fennecs                                     | 100000      |                                     205.61 ms | 3.918 ms | 4.812 ms | 204.94 ms |     3.48x slower |   0.28x | 193600400 B |       4.246x more |
| FlecsNET                                    | 100000      |                                     225.08 ms | 1.879 ms | 1.665 ms | 224.58 ms |     3.81x slower |   0.29x |       400 B | 113,983.520x less |
| Friflo                                      | 100000      |                                      13.83 ms | 0.489 ms | 1.373 ms |  13.30 ms |     4.39x faster |   0.53x |       400 B | 113,983.520x less |
| LeoEcs                                      | 100000      |                                      58.74 ms | 0.869 ms | 0.726 ms |  58.59 ms |     1.01x faster |   0.08x |       400 B | 113,983.520x less |
| LeoEcsLite                                  | 100000      |                                      23.16 ms | 0.973 ms | 2.759 ms |  22.61 ms |     2.60x faster |   0.35x |       400 B | 113,983.520x less |
| Morpeh                                      | 100000      |                                      48.08 ms | 1.255 ms | 3.660 ms |  47.06 ms |     1.24x faster |   0.13x |       400 B | 113,983.520x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**13.68 ms** | 0.584 ms | 1.686 ms |  13.67 ms | **4.36x faster** |   0.60x |        64 B | 712,397.000x less |
| TinyEcs                                     | 100000      |                                      35.09 ms | 0.988 ms | 2.704 ms |  34.70 ms |     1.70x faster |   0.18x |   1575160 B |      28.945x less |
| Xeno                                        | 100000      |                                      25.33 ms | 1.120 ms | 3.231 ms |  24.58 ms |     2.38x faster |   0.33x |       400 B | 113,983.520x less |

# Remove4Components

| Context                                     | EntityCount |                                          Mean |     Error |    StdDev |             Ratio | RatioSD |   Allocated |       Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|----------:|----------:|------------------:|--------:|------------:|------------------:|
| Arch                                        | 100000      |                                     46.675 ms | 0.9694 ms | 2.7969 ms |          baseline |         |  46051264 B |                   |
| DefaultECS                                  | 100000      |                                      3.991 ms | 0.0300 ms | 0.0266 ms |     11.70x faster |   0.70x |       400 B | 115,128.160x less |
| DragonECS                                   | 100000      |                                      3.505 ms | 0.0502 ms | 0.0866 ms |     13.32x faster |   0.85x |       400 B | 115,128.160x less |
| Fennecs                                     | 100000      |                                    179.631 ms | 3.4948 ms | 3.5889 ms |      3.86x slower |   0.25x | 193600400 B |       4.204x more |
| FlecsNET                                    | 100000      |                                    179.919 ms | 1.4471 ms | 1.2084 ms |      3.87x slower |   0.24x |       400 B | 115,128.160x less |
| Friflo                                      | 100000      |                                      6.883 ms | 0.0559 ms | 0.0436 ms |      6.78x faster |   0.41x |       400 B | 115,128.160x less |
| LeoEcs                                      | 100000      |                                     13.442 ms | 0.1031 ms | 0.0861 ms |      3.47x faster |   0.21x |       400 B | 115,128.160x less |
| LeoEcsLite                                  | 100000      |                                      7.266 ms | 0.1076 ms | 0.1007 ms |      6.43x faster |   0.39x |       400 B | 115,128.160x less |
| Morpeh                                      | 100000      |                                      4.558 ms | 0.0829 ms | 0.0735 ms |     10.24x faster |   0.63x |       400 B | 115,128.160x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**2.460 ms** | 0.0465 ms | 0.0434 ms | **18.98x faster** |   1.18x |       400 B | 115,128.160x less |
| TinyEcs                                     | 100000      |                                     18.991 ms | 0.3777 ms | 0.3154 ms |      2.46x faster |   0.15x |   1575160 B |      29.236x less |
| Xeno                                        | 100000      |                                      3.911 ms | 0.0565 ms | 0.0472 ms |     11.94x faster |   0.72x |       400 B | 115,128.160x less |

# SystemWith1ComponentMultipleComposition

| Context                                     | EntityCount | Padding |       Mean |    Error |    StdDev |     Median |         Ratio | RatioSD | Allocated | Alloc Ratio |
|---------------------------------------------|-------------|---------|-----------:|---------:|----------:|-----------:|--------------:|--------:|----------:|------------:|
| Arch                                        | 100000      | 0       |   245.4 μs |  3.91 μs |   3.26 μs |   244.3 μs |      baseline |         |     736 B |             |
| DefaultECS                                  | 100000      | 0       | 1,287.2 μs | 25.72 μs |  54.81 μs | 1,266.5 μs |  5.25x slower |   0.23x |     736 B |  1.00x more |
| DragonECS                                   | 100000      | 0       |   377.0 μs |  7.48 μs |   6.63 μs |   375.8 μs |  1.54x slower |   0.03x |     400 B |  1.84x less |
| Fennecs                                     | 100000      | 0       |   306.7 μs | 13.71 μs |  39.56 μs |   299.8 μs |  1.25x slower |   0.16x |     592 B |  1.24x less |
| FlecsNET                                    | 100000      | 0       |   849.8 μs | 16.11 μs |  19.79 μs |   841.7 μs |  3.46x slower |   0.09x |     400 B |  1.84x less |
| Friflo                                      | 100000      | 0       |   213.7 μs |  4.21 μs |   6.17 μs |   210.2 μs |  1.15x faster |   0.03x |     520 B |  1.42x less |
| LeoEcs                                      | 100000      | 0       |   214.0 μs |  4.27 μs |   9.47 μs |   210.5 μs |  1.15x faster |   0.05x |     400 B |  1.84x less |
| LeoEcsLite                                  | 100000      | 0       |   285.9 μs |  2.78 μs |   2.17 μs |   285.9 μs |  1.17x slower |   0.02x |     400 B |  1.84x less |
| Morpeh                                      | 100000      | 0       |   588.3 μs | 11.65 μs |  25.81 μs |   581.1 μs |  2.40x slower |   0.11x |     400 B |  1.84x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | 0       |   180.9 μs |  2.27 μs |   2.53 μs |   180.3 μs |  1.36x faster |   0.03x |     400 B |  1.84x less |
| TinyEcs                                     | 100000      | 0       |   922.5 μs | 37.02 μs | 107.42 μs |   924.6 μs |  3.76x slower |   0.44x |   13672 B | 18.58x more |
| Xeno                                        | 100000      | 0       |   274.2 μs |  4.50 μs |   3.99 μs |   273.1 μs |  1.12x slower |   0.02x |     400 B |  1.84x less |
|                                             |             |         |            |          |           |            |               |         |           |             |
| Arch                                        | 100000      | 10      |   254.5 μs |  4.55 μs |   7.73 μs |   251.8 μs |      baseline |         |     736 B |             |
| DefaultECS                                  | 100000      | 10      | 3,644.5 μs | 71.57 μs | 121.53 μs | 3,616.1 μs | 14.33x slower |   0.63x |     736 B |  1.00x more |
| DragonECS                                   | 100000      | 10      |   418.0 μs |  8.33 μs |  18.97 μs |   410.2 μs |  1.64x slower |   0.09x |     400 B |  1.84x less |
| Fennecs                                     | 100000      | 10      |   387.6 μs | 22.00 μs |  63.12 μs |   386.5 μs |  1.52x slower |   0.25x |     880 B |  1.20x more |
| FlecsNET                                    | 100000      | 10      |   867.6 μs | 17.32 μs |  22.52 μs |   863.4 μs |  3.41x slower |   0.13x |     400 B |  1.84x less |
| Friflo                                      | 100000      | 10      |   214.7 μs |  3.12 μs |   2.77 μs |   214.4 μs |  1.19x faster |   0.04x |     520 B |  1.42x less |
| LeoEcs                                      | 100000      | 10      |   270.0 μs |  4.64 μs |   3.62 μs |   269.8 μs |  1.06x slower |   0.03x |     400 B |  1.84x less |
| LeoEcsLite                                  | 100000      | 10      |   338.8 μs |  7.27 μs |  19.90 μs |   332.3 μs |  1.33x slower |   0.09x |     400 B |  1.84x less |
| Morpeh                                      | 100000      | 10      |   601.2 μs | 10.80 μs |  10.10 μs |   602.3 μs |  2.36x slower |   0.08x |     400 B |  1.84x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | 10      |   204.2 μs |  3.44 μs |   3.96 μs |   202.7 μs |  1.25x faster |   0.04x |     400 B |  1.84x less |
| TinyEcs                                     | 100000      | 10      |   980.7 μs | 51.79 μs | 146.06 μs |   962.8 μs |  3.86x slower |   0.58x |   13672 B | 18.58x more |
| Xeno                                        | 100000      | 10      |   320.4 μs |  3.70 μs |   3.28 μs |   320.4 μs |  1.26x slower |   0.04x |     400 B |  1.84x less |

# SystemWith1Component

| Context                                     | EntityCount | Padding |                                          Mean |    Error |    StdDev |     Median |         Ratio | RatioSD | Allocated | Alloc Ratio |
|---------------------------------------------|-------------|---------|----------------------------------------------:|---------:|----------:|-----------:|--------------:|--------:|----------:|------------:|
| Arch                                        | 100000      | 0       |                                      219.5 μs |  4.02 μs |   8.03 μs |   216.5 μs |      baseline |         |     736 B |             |
| DefaultECS                                  | 100000      | 0       |                                    1,324.2 μs | 44.43 μs | 119.35 μs | 1,277.0 μs |  6.04x slower |   0.58x |     736 B |  1.00x more |
| DragonECS                                   | 100000      | 0       |                                      377.4 μs |  6.37 μs |   5.96 μs |   375.4 μs |  1.72x slower |   0.07x |     400 B |  1.84x less |
| Fennecs                                     | 100000      | 0       |                                      310.0 μs | 17.42 μs |  50.83 μs |   299.4 μs |  1.41x slower |   0.24x |     880 B |  1.20x more |
| FlecsNET                                    | 100000      | 0       |                                      871.6 μs | 17.25 μs |  21.82 μs |   862.5 μs |  3.98x slower |   0.17x |     400 B |  1.84x less |
| Friflo                                      | 100000      | 0       |                                      212.8 μs |  4.19 μs |   4.66 μs |   211.0 μs |  1.03x faster |   0.04x |     232 B |  3.17x less |
| LeoEcs                                      | 100000      | 0       |                                      292.4 μs | 11.66 μs |  33.84 μs |   275.5 μs |  1.33x slower |   0.16x |     400 B |  1.84x less |
| LeoEcsLite                                  | 100000      | 0       |                                      288.5 μs |  5.52 μs |   5.43 μs |   287.2 μs |  1.32x slower |   0.05x |     400 B |  1.84x less |
| Morpeh                                      | 100000      | 0       |                                      585.2 μs | 11.70 μs |  20.80 μs |   581.5 μs |  2.67x slower |   0.13x |     400 B |  1.84x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | 0       | <span style="color: lightgreen;">**180.2 μs** |  1.76 μs |   1.56 μs |   179.7 μs |  1.22x faster |   0.05x |     400 B |  1.84x less |
| TinyEcs                                     | 100000      | 0       |                                      909.4 μs | 35.35 μs | 100.87 μs |   891.1 μs |  4.15x slower |   0.48x |   13672 B | 18.58x more |
| Xeno                                        | 100000      | 0       |                                      272.1 μs |  4.17 μs |   3.70 μs |   271.7 μs |  1.24x slower |   0.05x |     400 B |  1.84x less |
|                                             |             |         |                                               |          |           |            |               |         |           |             |
| Arch                                        | 100000      | 10      |                                      228.9 μs |  4.42 μs |   8.08 μs |   227.6 μs |      baseline |         |     736 B |             |
| DefaultECS                                  | 100000      | 10      |                                    4,460.1 μs | 88.16 μs | 202.56 μs | 4,412.2 μs | 19.50x slower |   1.09x |     736 B |  1.00x more |
| DragonECS                                   | 100000      | 10      |                                      452.6 μs | 12.75 μs |  35.75 μs |   438.5 μs |  1.98x slower |   0.17x |     400 B |  1.84x less |
| Fennecs                                     | 100000      | 10      |                                      388.8 μs | 28.06 μs |  79.60 μs |   372.5 μs |  1.70x slower |   0.35x |     880 B |  1.20x more |
| FlecsNET                                    | 100000      | 10      |                                      884.0 μs | 17.62 μs |  33.51 μs |   880.0 μs |  3.87x slower |   0.19x |     400 B |  1.84x less |
| Friflo                                      | 100000      | 10      |                                      215.3 μs |  3.70 μs |   8.04 μs |   213.3 μs |  1.06x faster |   0.05x |     520 B |  1.42x less |
| LeoEcs                                      | 100000      | 10      |                                      270.7 μs |  4.64 μs |   4.11 μs |   270.4 μs |  1.18x slower |   0.04x |     400 B |  1.84x less |
| LeoEcsLite                                  | 100000      | 10      |                                      400.3 μs | 18.40 μs |  51.60 μs |   379.5 μs |  1.75x slower |   0.23x |     400 B |  1.84x less |
| Morpeh                                      | 100000      | 10      |                                      587.4 μs | 19.78 μs |  55.80 μs |   593.9 μs |  2.57x slower |   0.26x |     400 B |  1.84x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | 10      | <span style="color: lightgreen;">**203.1 μs** |  3.20 μs |   2.67 μs |   201.7 μs |  1.13x faster |   0.04x |     400 B |  1.84x less |
| TinyEcs                                     | 100000      | 10      |                                    1,040.9 μs | 55.96 μs | 161.47 μs | 1,021.1 μs |  4.55x slower |   0.72x |   13672 B | 18.58x more |
| Xeno                                        | 100000      | 10      |                                      278.4 μs |  4.34 μs |   3.85 μs |   277.9 μs |  1.22x slower |   0.04x |     400 B |  1.84x less |

# SystemWith2ComponentsMultipleComposition

| Context                                     | EntityCount | Padding |                                          Mean |     Error |    StdDev |     Median |         Ratio | RatioSD | Allocated | Alloc Ratio |
|---------------------------------------------|-------------|---------|----------------------------------------------:|----------:|----------:|-----------:|--------------:|--------:|----------:|------------:|
| Arch                                        | 100000      | 0       |                                      301.9 μs |  14.33 μs |  42.02 μs |   307.4 μs |      baseline |         |     736 B |             |
| DefaultECS                                  | 100000      | 0       |                                    1,455.6 μs |  26.14 μs |  44.39 μs | 1,434.1 μs |  4.91x slower |   0.69x |     736 B |  1.00x more |
| DragonECS                                   | 100000      | 0       |                                      564.2 μs |   6.92 μs |   6.47 μs |   563.6 μs |  1.90x slower |   0.26x |     400 B |  1.84x less |
| Fennecs                                     | 100000      | 0       |                                      555.2 μs |  36.21 μs | 103.91 μs |   539.1 μs |  1.87x slower |   0.44x |     912 B |  1.24x more |
| FlecsNET                                    | 100000      | 0       |                                    1,394.4 μs |  27.89 μs |  49.57 μs | 1,378.9 μs |  4.71x slower |   0.66x |     400 B |  1.84x less |
| Friflo                                      | 100000      | 0       |                                      291.0 μs |   5.77 μs |   5.40 μs |   289.6 μs |  1.04x faster |   0.14x |     520 B |  1.42x less |
| LeoEcs                                      | 100000      | 0       |                                      394.3 μs |   5.25 μs |   4.10 μs |   393.3 μs |  1.33x slower |   0.18x |     400 B |  1.84x less |
| LeoEcsLite                                  | 100000      | 0       |                                      425.0 μs |   4.67 μs |   3.90 μs |   425.2 μs |  1.43x slower |   0.20x |     400 B |  1.84x less |
| Morpeh                                      | 100000      | 0       |                                    1,045.0 μs |  20.75 μs |  40.95 μs | 1,034.0 μs |  3.53x slower |   0.50x |     400 B |  1.84x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | 0       | <span style="color: lightgreen;">**353.7 μs** |   3.79 μs |   2.96 μs |   354.1 μs |  1.19x slower |   0.16x |     400 B |  1.84x less |
| TinyEcs                                     | 100000      | 0       |                                      947.1 μs |  55.85 μs | 162.03 μs |   950.4 μs |  3.20x slower |   0.70x |   14952 B | 20.32x more |
| Xeno                                        | 100000      | 0       |                                      444.7 μs |   8.51 μs |   8.36 μs |   443.6 μs |  1.50x slower |   0.21x |     400 B |  1.84x less |
|                                             |             |         |                                               |           |           |            |               |         |           |             |
| Arch                                        | 100000      | 10      |                                      331.1 μs |   6.61 μs |  12.41 μs |   327.9 μs |      baseline |         |     736 B |             |
| DefaultECS                                  | 100000      | 10      |                                    4,759.8 μs |  92.76 μs | 207.47 μs | 4,697.7 μs | 14.40x slower |   0.81x |     736 B |  1.00x more |
| DragonECS                                   | 100000      | 10      |                                      731.8 μs |  14.00 μs |  32.16 μs |   728.6 μs |  2.21x slower |   0.13x |     400 B |  1.84x less |
| Fennecs                                     | 100000      | 10      |                                      650.5 μs |  54.20 μs | 153.74 μs |   646.9 μs |  1.97x slower |   0.47x |     912 B |  1.24x more |
| FlecsNET                                    | 100000      | 10      |                                    1,391.2 μs |  27.69 μs |  45.50 μs | 1,378.8 μs |  4.21x slower |   0.20x |     400 B |  1.84x less |
| Friflo                                      | 100000      | 10      |                                      306.2 μs |   6.09 μs |  14.35 μs |   305.1 μs |  1.08x faster |   0.06x |     520 B |  1.42x less |
| LeoEcs                                      | 100000      | 10      |                                      477.5 μs |  12.83 μs |  35.54 μs |   467.1 μs |  1.44x slower |   0.12x |     400 B |  1.84x less |
| LeoEcsLite                                  | 100000      | 10      |                                    2,549.4 μs |  50.65 μs |  74.24 μs | 2,523.5 μs |  7.71x slower |   0.35x |     400 B |  1.84x less |
| Morpeh                                      | 100000      | 10      |                                    3,819.7 μs | 113.50 μs | 323.83 μs | 3,780.2 μs | 11.55x slower |   1.06x |     400 B |  1.84x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | 10      | <span style="color: lightgreen;">**920.4 μs** |  18.25 μs |  46.78 μs |   905.1 μs |  2.78x slower |   0.17x |     400 B |  1.84x less |
| TinyEcs                                     | 100000      | 10      |                                    1,126.6 μs |  63.46 μs | 175.84 μs | 1,104.0 μs |  3.41x slower |   0.54x |   14952 B | 20.32x more |
| Xeno                                        | 100000      | 10      |                                    1,906.2 μs |  52.85 μs | 145.55 μs | 1,874.9 μs |  5.77x slower |   0.48x |     400 B |  1.84x less |

# SystemWith2Components

| Context                                     | EntityCount | Padding |                                          Mean |    Error |    StdDev |     Median |         Ratio | RatioSD | Allocated | Alloc Ratio |
|---------------------------------------------|-------------|---------|----------------------------------------------:|---------:|----------:|-----------:|--------------:|--------:|----------:|------------:|
| Arch                                        | 100000      | 0       |                                      280.9 μs |  5.34 μs |   5.00 μs |   279.8 μs |      baseline |         |     736 B |             |
| DefaultECS                                  | 100000      | 0       |                                    1,448.2 μs | 28.68 μs |  68.16 μs | 1,417.5 μs |  5.16x slower |   0.26x |     736 B |  1.00x more |
| DragonECS                                   | 100000      | 0       |                                      567.0 μs |  9.91 μs |   8.79 μs |   566.5 μs |  2.02x slower |   0.05x |     400 B |  1.84x less |
| Fennecs                                     | 100000      | 0       |                                      500.6 μs | 28.43 μs |  82.03 μs |   488.2 μs |  1.78x slower |   0.29x |     880 B |  1.20x more |
| FlecsNET                                    | 100000      | 0       |                                    1,369.4 μs | 26.69 μs |  26.21 μs | 1,363.8 μs |  4.88x slower |   0.12x |     400 B |  1.84x less |
| Friflo                                      | 100000      | 0       |                                      265.7 μs |  8.08 μs |  22.39 μs |   269.0 μs |  1.07x faster |   0.10x |     184 B |  4.00x less |
| LeoEcs                                      | 100000      | 0       |                                      295.9 μs |  5.18 μs |  10.35 μs |   292.1 μs |  1.05x slower |   0.04x |     400 B |  1.84x less |
| LeoEcsLite                                  | 100000      | 0       |                                      424.7 μs |  3.02 μs |   2.35 μs |   425.1 μs |  1.51x slower |   0.03x |     400 B |  1.84x less |
| Morpeh                                      | 100000      | 0       |                                      966.6 μs | 19.30 μs |  22.97 μs |   969.0 μs |  3.44x slower |   0.10x |     400 B |  1.84x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | 0       | <span style="color: lightgreen;">**351.6 μs** |  3.92 μs |   3.48 μs |   350.2 μs |  1.25x slower |   0.02x |     400 B |  1.84x less |
| TinyEcs                                     | 100000      | 0       |                                      924.0 μs | 40.45 μs | 112.09 μs |   923.4 μs |  3.29x slower |   0.40x |   14400 B | 19.57x more |
| Xeno                                        | 100000      | 0       |                                      427.7 μs |  8.34 μs |  10.54 μs |   424.7 μs |  1.52x slower |   0.04x |     112 B |  6.57x less |
|                                             |             |         |                                               |          |           |            |               |         |           |             |
| Arch                                        | 100000      | 10      |                                      292.6 μs |  5.44 μs |  11.83 μs |   289.6 μs |      baseline |         |     736 B |             |
| DefaultECS                                  | 100000      | 10      |                                    4,761.7 μs | 90.47 μs | 192.80 μs | 4,679.4 μs | 16.30x slower |   0.90x |     736 B |  1.00x more |
| DragonECS                                   | 100000      | 10      |                                      768.4 μs | 17.65 μs |  48.02 μs |   760.8 μs |  2.63x slower |   0.19x |     400 B |  1.84x less |
| Fennecs                                     | 100000      | 10      |                                      603.5 μs | 55.94 μs | 156.86 μs |   567.8 μs |  2.07x slower |   0.54x |     880 B |  1.20x more |
| FlecsNET                                    | 100000      | 10      |                                    1,379.8 μs | 20.48 μs |  17.10 μs | 1,378.1 μs |  4.72x slower |   0.19x |     400 B |  1.84x less |
| Friflo                                      | 100000      | 10      |                                      271.4 μs |  4.97 μs |  10.90 μs |   268.6 μs |  1.08x faster |   0.06x |     520 B |  1.42x less |
| LeoEcs                                      | 100000      | 10      |                                      464.4 μs |  8.84 μs |  14.78 μs |   460.5 μs |  1.59x slower |   0.08x |     400 B |  1.84x less |
| LeoEcsLite                                  | 100000      | 10      |                                    2,508.3 μs | 40.17 μs |  35.61 μs | 2,496.7 μs |  8.59x slower |   0.35x |     400 B |  1.84x less |
| Morpeh                                      | 100000      | 10      |                                    1,284.9 μs | 54.34 μs | 149.66 μs | 1,235.4 μs |  4.40x slower |   0.54x |     400 B |  1.84x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | 10      | <span style="color: lightgreen;">**872.7 μs** | 17.31 μs |  23.69 μs |   864.8 μs |  2.99x slower |   0.14x |     400 B |  1.84x less |
| TinyEcs                                     | 100000      | 10      |                                    1,130.2 μs | 62.75 μs | 175.96 μs | 1,086.7 μs |  3.87x slower |   0.62x |   14400 B | 19.57x more |
| Xeno                                        | 100000      | 10      |                                      728.2 μs | 30.18 μs |  87.08 μs |   694.1 μs |  2.49x slower |   0.31x |     400 B |  1.84x less |

# SystemWith3ComponentsMultipleComposition

| Context                                     | EntityCount | Padding |                                            Mean |     Error |      StdDev |     Median |         Ratio | RatioSD | Allocated | Alloc Ratio |
|---------------------------------------------|-------------|---------|------------------------------------------------:|----------:|------------:|-----------:|--------------:|--------:|----------:|------------:|
| Arch                                        | 100000      | 0       |                                        300.0 μs |  16.88 μs |    48.70 μs |   306.1 μs |      baseline |         |     736 B |             |
| DefaultECS                                  | 100000      | 0       |                                      3,886.8 μs | 990.35 μs | 2,920.08 μs | 1,743.7 μs | 13.28x slower |  10.24x |     736 B |  1.00x more |
| DragonECS                                   | 100000      | 0       |                                        789.5 μs |  15.73 μs |    24.94 μs |   778.8 μs |  2.70x slower |   0.42x |     400 B |  1.84x less |
| Fennecs                                     | 100000      | 0       |                                        553.3 μs |  41.56 μs |   119.24 μs |   546.4 μs |  1.89x slower |   0.50x |     912 B |  1.24x more |
| FlecsNET                                    | 100000      | 0       |                                      1,862.6 μs |  35.91 μs |    41.35 μs | 1,859.6 μs |  6.36x slower |   0.97x |     400 B |  1.84x less |
| Friflo                                      | 100000      | 0       |                                        438.9 μs |   8.72 μs |     7.73 μs |   436.1 μs |  1.50x slower |   0.23x |     232 B |  3.17x less |
| LeoEcs                                      | 100000      | 0       |                                        550.8 μs |  10.82 μs |    14.81 μs |   545.8 μs |  1.88x slower |   0.29x |     400 B |  1.84x less |
| LeoEcsLite                                  | 100000      | 0       |                                        561.8 μs |   9.54 μs |    10.60 μs |   560.5 μs |  1.92x slower |   0.29x |     400 B |  1.84x less |
| Morpeh                                      | 100000      | 0       |                                      1,433.2 μs |  59.57 μs |   172.84 μs | 1,437.4 μs |  4.90x slower |   0.95x |     400 B |  1.84x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | 0       |   <span style="color: lightgreen;">**485.3 μs** |   2.60 μs |     2.30 μs |   486.0 μs |  1.66x slower |   0.25x |     400 B |  1.84x less |
| TinyEcs                                     | 100000      | 0       |                                        997.9 μs |  42.03 μs |   119.24 μs |   991.3 μs |  3.41x slower |   0.66x |   15344 B | 20.85x more |
| Xeno                                        | 100000      | 0       |                                        526.6 μs |   5.50 μs |     4.59 μs |   526.0 μs |  1.80x slower |   0.27x |     400 B |  1.84x less |
|                                             |             |         |                                                 |           |             |            |               |         |           |             |
| Arch                                        | 100000      | 10      |                                        367.8 μs |  11.37 μs |    31.50 μs |   357.4 μs |      baseline |         |     736 B |             |
| DefaultECS                                  | 100000      | 10      |                                      5,125.6 μs | 102.49 μs |   268.20 μs | 5,038.5 μs | 14.03x slower |   1.33x |     736 B |  1.00x more |
| DragonECS                                   | 100000      | 10      |                                        989.0 μs |  26.22 μs |    70.87 μs |   972.4 μs |  2.71x slower |   0.29x |     400 B |  1.84x less |
| Fennecs                                     | 100000      | 10      |                                        753.1 μs |  58.91 μs |   169.04 μs |   719.7 μs |  2.06x slower |   0.49x |     912 B |  1.24x more |
| FlecsNET                                    | 100000      | 10      |                                      1,984.7 μs |  72.82 μs |   208.93 μs | 1,876.9 μs |  5.43x slower |   0.72x |     400 B |  1.84x less |
| Friflo                                      | 100000      | 10      |                                        448.2 μs |   8.23 μs |    20.04 μs |   441.9 μs |  1.23x slower |   0.11x |     520 B |  1.42x less |
| LeoEcs                                      | 100000      | 10      |                                        593.4 μs |  11.88 μs |    30.02 μs |   587.1 μs |  1.62x slower |   0.15x |     400 B |  1.84x less |
| LeoEcsLite                                  | 100000      | 10      |                                      2,915.8 μs |  95.57 μs |   275.75 μs | 2,780.1 μs |  7.98x slower |   0.98x |     400 B |  1.84x less |
| Morpeh                                      | 100000      | 10      |                                      6,137.6 μs | 289.62 μs |   853.94 μs | 5,826.4 μs | 16.80x slower |   2.69x |     400 B |  1.84x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | 10      | <span style="color: lightgreen;">**1,072.5 μs** |  58.83 μs |   171.62 μs | 1,019.9 μs |  2.94x slower |   0.52x |     400 B |  1.84x less |
| TinyEcs                                     | 100000      | 10      |                                      1,120.7 μs |  52.59 μs |   147.46 μs | 1,097.9 μs |  3.07x slower |   0.47x |   15344 B | 20.85x more |
| Xeno                                        | 100000      | 10      |                                      1,785.7 μs |  87.53 μs |   241.07 μs | 1,693.1 μs |  4.89x slower |   0.76x |     400 B |  1.84x less |

# SystemWith3Components

| Context                                     | EntityCount | Padding |                                          Mean |    Error |    StdDev |     Median |         Ratio | RatioSD | Allocated | Alloc Ratio |
|---------------------------------------------|-------------|---------|----------------------------------------------:|---------:|----------:|-----------:|--------------:|--------:|----------:|------------:|
| Arch                                        | 100000      | 0       |                                      280.7 μs |  5.46 μs |   5.61 μs |   280.6 μs |      baseline |         |     736 B |             |
| DefaultECS                                  | 100000      | 0       |                                    1,683.8 μs | 14.98 μs |  13.28 μs | 1,683.0 μs |  6.00x slower |   0.12x |     736 B |  1.00x more |
| DragonECS                                   | 100000      | 0       |                                      770.8 μs | 14.59 μs |  14.33 μs |   768.8 μs |  2.75x slower |   0.07x |     400 B |  1.84x less |
| Fennecs                                     | 100000      | 0       |                                      575.6 μs | 35.03 μs | 101.64 μs |   588.6 μs |  2.05x slower |   0.36x |     880 B |  1.20x more |
| FlecsNET                                    | 100000      | 0       |                                    1,857.0 μs | 22.17 μs |  17.31 μs | 1,859.2 μs |  6.62x slower |   0.14x |     400 B |  1.84x less |
| Friflo                                      | 100000      | 0       |                                      402.8 μs |  6.81 μs |   7.00 μs |   400.0 μs |  1.44x slower |   0.04x |     520 B |  1.42x less |
| LeoEcs                                      | 100000      | 0       |                                      556.3 μs | 11.02 μs |  12.69 μs |   556.9 μs |  1.98x slower |   0.06x |     400 B |  1.84x less |
| LeoEcsLite                                  | 100000      | 0       |                                      551.8 μs |  4.72 μs |   3.68 μs |   551.2 μs |  1.97x slower |   0.04x |     400 B |  1.84x less |
| Morpeh                                      | 100000      | 0       |                                    1,359.0 μs | 21.91 μs |  18.30 μs | 1,362.2 μs |  4.84x slower |   0.11x |   12736 B | 17.30x more |
| <span style="color: white;">**[StaticEcs]** | **100000**  | 0       | <span style="color: lightgreen;">**491.3 μs** |  8.55 μs |  10.18 μs |   488.3 μs |  1.75x slower |   0.05x |     400 B |  1.84x less |
| TinyEcs                                     | 100000      | 0       |                                      913.9 μs | 99.86 μs | 291.30 μs |   902.2 μs |  3.26x slower |   1.04x |   14768 B | 20.07x more |
| Xeno                                        | 100000      | 0       |                                      516.8 μs |  4.94 μs |   3.86 μs |   516.8 μs |  1.84x slower |   0.04x |     400 B |  1.84x less |
|                                             |             |         |                                               |          |           |            |               |         |           |             |
| Arch                                        | 100000      | 10      |                                      292.3 μs |  4.81 μs |  12.60 μs |   288.5 μs |      baseline |         |     736 B |             |
| DefaultECS                                  | 100000      | 10      |                                    5,080.9 μs | 99.81 μs | 194.68 μs | 5,034.5 μs | 17.41x slower |   0.96x |     736 B |  1.00x more |
| DragonECS                                   | 100000      | 10      |                                    1,005.9 μs | 28.24 μs |  77.77 μs |   984.4 μs |  3.45x slower |   0.30x |     400 B |  1.84x less |
| Fennecs                                     | 100000      | 10      |                                      664.4 μs | 53.79 μs | 147.26 μs |   636.3 μs |  2.28x slower |   0.51x |     880 B |  1.20x more |
| FlecsNET                                    | 100000      | 10      |                                    1,875.4 μs | 36.65 μs |  36.00 μs | 1,867.2 μs |  6.43x slower |   0.28x |     400 B |  1.84x less |
| Friflo                                      | 100000      | 10      |                                      415.1 μs |  8.16 μs |  17.22 μs |   410.8 μs |  1.42x slower |   0.08x |     520 B |  1.42x less |
| LeoEcs                                      | 100000      | 10      |                                      619.3 μs | 12.33 μs |  25.46 μs |   616.2 μs |  2.12x slower |   0.12x |     400 B |  1.84x less |
| LeoEcsLite                                  | 100000      | 10      |                                    3,343.2 μs | 63.95 μs |  80.88 μs | 3,320.5 μs | 11.46x slower |   0.53x |     400 B |  1.84x less |
| Morpeh                                      | 100000      | 10      |                                    1,571.7 μs | 47.58 μs | 134.99 μs | 1,567.7 μs |  5.39x slower |   0.51x |     400 B |  1.84x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | 10      | <span style="color: lightgreen;">**849.5 μs** | 30.81 μs |  85.37 μs |   820.1 μs |  2.91x slower |   0.31x |     400 B |  1.84x less |
| TinyEcs                                     | 100000      | 10      |                                    1,166.0 μs | 64.96 μs | 187.44 μs | 1,154.5 μs |  4.00x slower |   0.66x |   14768 B | 20.07x more |
| Xeno                                        | 100000      | 10      |                                      906.8 μs | 29.00 μs |  83.20 μs |   888.5 μs |  3.11x slower |   0.31x |     400 B |  1.84x less |

# ThreeAddOneComponent

| Context                                     | EntityCount |                                          Mean |       Error |      StdDev |      Median |             Ratio | RatioSD |  Allocated |     Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|------------:|------------:|------------:|------------------:|--------:|-----------:|----------------:|
| Arch                                        | 100000      |                                   11,980.7 μs |   175.82 μs |   155.86 μs | 11,955.0 μs |          baseline |         |     1072 B |                 |
| DefaultECS                                  | 100000      |                                    1,389.1 μs |    25.30 μs |    27.07 μs |  1,388.1 μs |      8.63x faster |   0.19x |      400 B |      2.68x less |
| DragonECS                                   | 100000      |                                      896.0 μs |    20.61 μs |    56.78 μs |    880.4 μs |     13.42x faster |   0.82x |      400 B |      2.68x less |
| Fennecs                                     | 100000      |                                   64,875.5 μs |   993.29 μs |   880.52 μs | 64,550.0 μs |      5.42x slower |   0.10x | 84000400 B | 78,358.58x more |
| FlecsNET                                    | 100000      |                                   92,459.6 μs | 1,315.39 μs | 1,166.05 μs | 92,630.3 μs |      7.72x slower |   0.13x |      400 B |      2.68x less |
| Friflo                                      | 100000      |                                    3,167.5 μs |    24.53 μs |    20.49 μs |  3,165.3 μs |      3.78x faster |   0.05x |      400 B |      2.68x less |
| LeoEcs                                      | 100000      |                                    3,754.7 μs |    50.63 μs |    42.28 μs |  3,749.5 μs |      3.19x faster |   0.05x |      400 B |      2.68x less |
| LeoEcsLite                                  | 100000      |                                    1,630.2 μs |    16.13 μs |    13.47 μs |  1,625.1 μs |      7.35x faster |   0.11x |      400 B |      2.68x less |
| Morpeh                                      | 100000      |                                    3,021.0 μs |    76.64 μs |   218.67 μs |  2,963.2 μs |      3.99x faster |   0.28x |      400 B |      2.68x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**343.1 μs** |     6.48 μs |     7.20 μs |    341.9 μs | **34.94x faster** |   0.84x |      400 B |      2.68x less |
| TinyEcs                                     | 100000      |                                    3,823.6 μs |    75.46 μs |    66.90 μs |  3,817.3 μs |      3.13x faster |   0.07x |  2400400 B |  2,239.18x more |
| Xeno                                        | 100000      |                                    2,514.1 μs |    76.90 μs |   203.93 μs |  2,453.1 μs |      4.79x faster |   0.34x |      400 B |      2.68x less |

# ThreeRemoveOneComponent

| Context                                     | EntityCount |                                          Mean |       Error |      StdDev |      Median |             Ratio | RatioSD |  Allocated |      Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|------------:|------------:|------------:|------------------:|--------:|-----------:|-----------------:|
| Arch                                        | 100000      |                                   38,690.5 μs |   676.50 μs |   599.70 μs | 38,576.3 μs |          baseline |         | 24808464 B |                  |
| DefaultECS                                  | 100000      |                                    1,078.5 μs |    21.33 μs |    18.91 μs |  1,073.5 μs |     35.89x faster |   0.81x |      400 B | 62,021.160x less |
| DragonECS                                   | 100000      |                                      978.1 μs |    21.82 μs |    57.48 μs |    965.1 μs |     39.68x faster |   2.17x |      400 B | 62,021.160x less |
| Fennecs                                     | 100000      |                                   53,195.8 μs | 1,061.02 μs | 2,642.30 μs | 52,743.3 μs |      1.38x slower |   0.07x | 58400400 B |      2.354x more |
| FlecsNET                                    | 100000      |                                   71,021.4 μs | 1,134.67 μs |   885.87 μs | 70,760.4 μs |      1.84x slower |   0.04x |      400 B | 62,021.160x less |
| Friflo                                      | 100000      |                                    2,146.9 μs |     9.63 μs |     8.54 μs |  2,146.7 μs |     18.02x faster |   0.28x |      400 B | 62,021.160x less |
| LeoEcs                                      | 100000      |                                    6,798.6 μs |   130.15 μs |   159.84 μs |  6,793.6 μs |      5.69x faster |   0.16x |      400 B | 62,021.160x less |
| LeoEcsLite                                  | 100000      |                                    3,002.6 μs |    43.14 μs |    38.24 μs |  2,992.7 μs |     12.89x faster |   0.25x |      400 B | 62,021.160x less |
| Morpeh                                      | 100000      |                                    3,305.8 μs |   127.34 μs |   354.98 μs |  3,205.7 μs |     11.83x faster |   1.19x |      400 B | 62,021.160x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**460.6 μs** |     9.16 μs |    26.12 μs |    452.4 μs | **84.26x faster** |   4.74x |      400 B | 62,021.160x less |
| TinyEcs                                     | 100000      |                                      581.1 μs |     8.61 μs |    21.90 μs |    578.6 μs |     66.67x faster |   2.59x |      400 B | 62,021.160x less |
| Xeno                                        | 100000      |                                    2,587.1 μs |    27.07 μs |    38.83 μs |  2,583.8 μs |     14.96x faster |   0.31x |  1045224 B |     23.735x less |

# ThreeRemoveTwoComponents

| Context                                     | EntityCount |                                          Mean |       Error |      StdDev |             Ratio | RatioSD |  Allocated |      Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|------------:|------------:|------------------:|--------:|-----------:|-----------------:|
| Arch                                        | 100000      |                                   34,895.0 μs |   349.00 μs |   309.38 μs |          baseline |         | 24808464 B |                  |
| DefaultECS                                  | 100000      |                                    2,015.6 μs |    11.43 μs |     9.54 μs |     17.31x faster |   0.17x |      400 B | 62,021.160x less |
| DragonECS                                   | 100000      |                                    5,003.9 μs |    56.74 μs |    47.38 μs |      6.97x faster |   0.09x |      400 B | 62,021.160x less |
| Fennecs                                     | 100000      |                                   88,790.2 μs | 1,230.48 μs | 1,150.99 μs |      2.54x slower |   0.04x | 96800400 B |      3.902x more |
| FlecsNET                                    | 100000      |                                  124,169.3 μs | 1,530.78 μs | 1,431.89 μs |      3.56x slower |   0.05x |      400 B | 62,021.160x less |
| Friflo                                      | 100000      |                                    3,538.1 μs |    27.30 μs |    24.20 μs |      9.86x faster |   0.11x |      400 B | 62,021.160x less |
| LeoEcs                                      | 100000      |                                    9,694.6 μs |   102.64 μs |    90.98 μs |      3.60x faster |   0.05x |      400 B | 62,021.160x less |
| LeoEcsLite                                  | 100000      |                                    5,614.7 μs |    56.05 μs |    52.43 μs |      6.22x faster |   0.08x |      400 B | 62,021.160x less |
| Morpeh                                      | 100000      |                                    3,294.4 μs |    64.32 μs |   103.87 μs |     10.60x faster |   0.33x |      400 B | 62,021.160x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**951.0 μs** |     9.21 μs |     8.17 μs | **36.69x faster** |   0.44x |      400 B | 62,021.160x less |
| TinyEcs                                     | 100000      |                                    1,551.7 μs |    26.68 μs |    23.65 μs |     22.49x faster |   0.38x |      400 B | 62,021.160x less |
| Xeno                                        | 100000      |                                    2,956.1 μs |    19.57 μs |    34.78 μs |     11.81x faster |   0.17x |      400 B | 62,021.160x less |

# TwoAddOneComponent

| Context                                     | EntityCount |                                          Mean |       Error |      StdDev |             Ratio | RatioSD |  Allocated |     Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|------------:|------------:|------------------:|--------:|-----------:|----------------:|
| Arch                                        | 100000      |                                    9,223.3 μs |   102.57 μs |    95.94 μs |          baseline |         |     1072 B |                 |
| DefaultECS                                  | 100000      |                                    1,352.8 μs |    26.31 μs |    29.25 μs |      6.82x faster |   0.16x |      400 B |      2.68x less |
| DragonECS                                   | 100000      |                                      858.7 μs |    17.16 μs |    42.09 μs |     10.77x faster |   0.53x |      400 B |      2.68x less |
| Fennecs                                     | 100000      |                                   57,904.0 μs | 1,143.04 μs | 1,122.62 μs |      6.28x slower |   0.13x | 71200400 B | 66,418.28x more |
| FlecsNET                                    | 100000      |                                   88,785.1 μs | 1,646.35 μs | 1,459.44 μs |      9.63x slower |   0.18x |      400 B |      2.68x less |
| Friflo                                      | 100000      |                                    2,659.5 μs |    23.04 μs |    20.42 μs |      3.47x faster |   0.04x |      400 B |      2.68x less |
| LeoEcs                                      | 100000      |                                    3,283.2 μs |    29.12 μs |    22.74 μs |      2.81x faster |   0.03x |      400 B |      2.68x less |
| LeoEcsLite                                  | 100000      |                                    1,531.3 μs |    27.32 μs |    22.82 μs |      6.02x faster |   0.10x |      400 B |      2.68x less |
| Morpeh                                      | 100000      |                                    3,179.1 μs |    75.37 μs |   216.26 μs |      2.91x faster |   0.19x |      400 B |      2.68x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**330.0 μs** |     4.70 μs |     8.24 μs | **27.97x faster** |   0.74x |      400 B |      2.68x less |
| TinyEcs                                     | 100000      |                                    3,671.5 μs |    56.84 μs |    47.47 μs |      2.51x faster |   0.04x |  2400400 B |  2,239.18x more |
| Xeno                                        | 100000      |                                    2,542.5 μs |    59.03 μs |   159.59 μs |      3.64x faster |   0.22x |  1045232 B |    975.03x more |

# TwoAddTwoComponents

| Context                                     | EntityCount |                                          Mean |     Error |    StdDev |             Ratio | RatioSD |   Allocated |      Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|----------:|----------:|------------------:|--------:|------------:|-----------------:|
| Arch                                        | 100000      |                                   10,473.2 μs |  74.68 μs |  58.30 μs |          baseline |         |      1072 B |                  |
| DefaultECS                                  | 100000      |                                    2,612.2 μs |  24.66 μs |  20.59 μs |      4.01x faster |   0.04x |       400 B |       2.68x less |
| DragonECS                                   | 100000      |                                    1,312.5 μs |  25.67 μs |  44.96 μs |      7.99x faster |   0.26x |       400 B |       2.68x less |
| Fennecs                                     | 100000      |                                  117,630.6 μs | 560.49 μs | 468.04 μs |     11.23x slower |   0.07x | 155200400 B | 144,776.49x more |
| FlecsNET                                    | 100000      |                                  164,428.1 μs | 605.00 μs | 536.32 μs |     15.70x slower |   0.10x |       400 B |       2.68x less |
| Friflo                                      | 100000      |                                    5,220.3 μs |  26.95 μs |  25.21 μs |      2.01x faster |   0.01x |       400 B |       2.68x less |
| LeoEcs                                      | 100000      |                                    6,824.6 μs |  75.31 μs |  70.45 μs |      1.53x faster |   0.02x |       400 B |       2.68x less |
| LeoEcsLite                                  | 100000      |                                    3,809.0 μs |  25.71 μs |  20.07 μs |      2.75x faster |   0.02x |       400 B |       2.68x less |
| Morpeh                                      | 100000      |                                    3,491.7 μs |  69.78 μs |  93.15 μs |      3.00x faster |   0.08x |       400 B |       2.68x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**681.9 μs** |   7.44 μs |   6.21 μs | **15.36x faster** |   0.16x |       400 B |       2.68x less |
| TinyEcs                                     | 100000      |                                    6,940.3 μs | 137.10 μs | 152.39 μs |      1.51x faster |   0.03x |   4800400 B |   4,477.99x more |
| Xeno                                        | 100000      |                                    3,059.7 μs |  55.24 μs |  89.20 μs |      3.43x faster |   0.10x |   1045232 B |     975.03x more |

# TwoRemoveOneComponent

| Context                                     | EntityCount |                                          Mean |       Error |    StdDev |      Median |             Ratio | RatioSD |  Allocated |      Alloc Ratio |
|---------------------------------------------|-------------|----------------------------------------------:|------------:|----------:|------------:|------------------:|--------:|-----------:|-----------------:|
| Arch                                        | 100000      |                                   32,465.9 μs |   447.77 μs | 396.94 μs | 32,413.9 μs |          baseline |         | 24001688 B |                  |
| DefaultECS                                  | 100000      |                                    1,054.7 μs |    20.12 μs |  23.95 μs |  1,047.2 μs |     30.80x faster |   0.76x |      400 B | 60,004.220x less |
| DragonECS                                   | 100000      |                                    1,007.8 μs |    71.64 μs | 186.21 μs |    949.2 μs |     32.91x faster |   3.92x |      400 B | 60,004.220x less |
| Fennecs                                     | 100000      |                                   39,575.8 μs |   279.18 μs | 247.49 μs | 39,537.5 μs |      1.22x slower |   0.02x | 44000400 B |      1.833x more |
| FlecsNET                                    | 100000      |                                   72,528.5 μs | 1,057.88 μs | 989.54 μs | 72,315.8 μs |      2.23x slower |   0.04x |      400 B | 60,004.220x less |
| Friflo                                      | 100000      |                                    1,734.4 μs |    32.85 μs |  35.15 μs |  1,724.7 μs |     18.73x faster |   0.42x |      400 B | 60,004.220x less |
| LeoEcs                                      | 100000      |                                    6,314.1 μs |   125.93 μs | 154.66 μs |  6,294.5 μs |      5.14x faster |   0.14x |      400 B | 60,004.220x less |
| LeoEcsLite                                  | 100000      |                                    2,508.1 μs |    42.71 μs |  37.86 μs |  2,498.8 μs |     12.95x faster |   0.24x |      400 B | 60,004.220x less |
| Morpeh                                      | 100000      |                                    2,925.0 μs |    58.42 μs | 157.93 μs |  2,875.1 μs |     11.13x faster |   0.59x |      400 B | 60,004.220x less |
| <span style="color: white;">**[StaticEcs]** | **100000**  | <span style="color: lightgreen;">**459.4 μs** |     5.43 μs |   4.24 μs |    458.8 μs | **70.68x faster** |   1.04x |      400 B | 60,004.220x less |
| TinyEcs                                     | 100000      |                                      720.3 μs |    13.99 μs |  13.74 μs |    715.2 μs |     45.09x faster |   0.97x |      400 B | 60,004.220x less |
| Xeno                                        | 100000      |                                    2,462.2 μs |    48.77 μs |  78.75 μs |  2,444.6 μs |     13.20x faster |   0.41x |      400 B | 60,004.220x less |

