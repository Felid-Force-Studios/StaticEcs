## Benchmark - [source code](https://github.com/blackbone/other-ecs-benchmarks/pull/9/commits/cb081bd943c71598921084e6098813932b986768)

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19045.5608/22H2/2022Update)
AMD Ryzen 7 4800H with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.202
[Host]     : .NET 9.0.3 (9.0.325.11113), X64 RyuJIT AVX2
Job-TUUGWI : .NET 9.0.3 (9.0.325.11113), X64 RyuJIT AVX2

Method=Run InvocationCount=1 RunStrategy=Throughput  
UnrollFactor=1

# Add1ComponentRandomOrder

| Context                 | EntityCount |        Mean |       Error |      StdDev |      Median |  Allocated |
|-------------------------|-------------|------------:|------------:|------------:|------------:|-----------:|
| 📍 **StaticEcsContext** | 100000      |    864.2 μs |    20.98 μs |    59.16 μs |    866.4 μs |       64 B |
| MassiveEcsContext       | 100000      |    972.2 μs |    16.95 μs |    37.56 μs |    968.8 μs |      400 B |
| FrifloContext           | 100000      |  4,128.4 μs |   284.37 μs |   811.32 μs |  4,008.2 μs |      400 B |
| DragonECSContext        | 100000      |  4,129.5 μs |   128.97 μs |   361.66 μs |  4,038.3 μs |      400 B |
| LeoEcsLiteContext       | 100000      |  5,117.2 μs |   325.94 μs |   935.19 μs |  4,874.4 μs |      400 B |
| XenoContext             | 100000      |  8,378.0 μs |   480.55 μs | 1,378.80 μs |  7,984.9 μs |   983688 B |
| ArchContext             | 100000      |  8,728.3 μs |   459.31 μs | 1,332.54 μs |  8,546.9 μs |     1120 B |
| DefaultECSContext       | 100000      | 11,371.9 μs |   426.54 μs | 1,223.81 μs | 11,125.3 μs |       64 B |
| TinyEcsContext          | 100000      | 13,290.3 μs |   265.38 μs |   703.75 μs | 13,170.9 μs |  2400400 B |
| MorpehContext           | 100000      | 16,875.9 μs |   337.39 μs |   673.81 μs | 16,688.8 μs |      400 B |
| LeoEcsContext           | 100000      | 17,501.7 μs |   347.94 μs |   710.74 μs | 17,248.6 μs |      400 B |
| FennecsContext          | 100000      | 45,548.8 μs |   796.16 μs | 1,006.89 μs | 45,127.4 μs | 49600400 B |
| FlecsNETContext         | 100000      | 87,363.8 μs | 1,698.43 μs | 2,324.83 μs | 86,303.9 μs |      400 B |

# Add1Component

| Context                 | EntityCount |        Mean |     Error |      StdDev |      Median |  Allocated |
|-------------------------|-------------|------------:|----------:|------------:|------------:|-----------:|
| 📍 **StaticEcsContext** | 100000      |    396.8 μs |   7.56 μs |     6.31 μs |    395.1 μs |      400 B |
| MassiveEcsContext       | 100000      |    730.0 μs |  12.96 μs |    30.56 μs |    720.8 μs |      400 B |
| DragonECSContext        | 100000      |  1,030.7 μs |  47.10 μs |   138.14 μs |    988.2 μs |      400 B |
| DefaultECSContext       | 100000      |  1,479.2 μs |  52.74 μs |   148.77 μs |  1,422.7 μs |      400 B |
| LeoEcsLiteContext       | 100000      |  1,573.4 μs |  13.25 μs |    21.40 μs |  1,567.0 μs |      400 B |
| FrifloContext           | 100000      |  1,633.0 μs |  15.78 μs |    26.79 μs |  1,623.7 μs |      400 B |
| MorpehContext           | 100000      |  2,599.1 μs |  51.91 μs |   115.03 μs |  2,588.7 μs |      400 B |
| XenoContext             | 100000      |  3,275.9 μs |  64.86 μs |   116.96 μs |  3,239.6 μs |   983688 B |
| LeoEcsContext           | 100000      |  3,544.1 μs |  70.59 μs |   109.90 μs |  3,510.4 μs |      400 B |
| TinyEcsContext          | 100000      |  4,233.2 μs | 142.35 μs |   384.86 μs |  4,129.6 μs |  2400064 B |
| ArchContext             | 100000      |  4,453.7 μs |  88.77 μs |   209.25 μs |  4,371.5 μs |     1744 B |
| FennecsContext          | 100000      | 36,921.7 μs | 820.32 μs | 2,392.92 μs | 37,072.1 μs | 49600400 B |
| FlecsNETContext         | 100000      | 61,170.6 μs | 474.07 μs |   395.87 μs | 61,250.7 μs |      400 B |

# Add1RandomComponentRandomOrder

| Context                 | EntityCount |       Mean |     Error |    StdDev |     Median |  Allocated |
|-------------------------|-------------|-----------:|----------:|----------:|-----------:|-----------:|
| 📍 **StaticEcsContext** | 100000      |   2.615 ms | 0.1292 ms | 0.3536 ms |   2.507 ms |      400 B |
| MassiveEcsContext       | 100000      |   4.909 ms | 0.2931 ms | 0.8268 ms |   4.576 ms |      400 B |
| FrifloContext           | 100000      |   6.195 ms | 0.2789 ms | 0.8001 ms |   6.127 ms |      112 B |
| ArchContext             | 100000      |   9.415 ms | 0.1998 ms | 0.5700 ms |   9.288 ms |     1744 B |
| DragonECSContext        | 100000      |  10.782 ms | 0.3259 ms | 0.9299 ms |  10.529 ms |      400 B |
| LeoEcsLiteContext       | 100000      |  11.056 ms | 0.2199 ms | 0.5352 ms |  10.971 ms |      400 B |
| TinyEcsContext          | 100000      |  14.087 ms | 0.3425 ms | 0.9435 ms |  13.936 ms |  2400064 B |
| DefaultECSContext       | 100000      |  14.172 ms | 0.2742 ms | 0.4188 ms |  14.122 ms |      400 B |
| XenoContext             | 100000      |  17.195 ms | 0.9590 ms | 2.6734 ms |  16.751 ms |   787632 B |
| MorpehContext           | 100000      |  22.328 ms | 0.3432 ms | 0.3042 ms |  22.333 ms |      400 B |
| LeoEcsContext           | 100000      |  24.819 ms | 0.4916 ms | 1.2864 ms |  24.747 ms |       64 B |
| FennecsContext          | 100000      |  49.841 ms | 0.9961 ms | 1.6366 ms |  49.842 ms | 49600400 B |
| FlecsNETContext         | 100000      | 126.121 ms | 0.4787 ms | 0.3737 ms | 126.187 ms |      400 B |

# Add1RandomComponent

| Context                 | EntityCount |       Mean |     Error |    StdDev |     Median |  Allocated |
|-------------------------|-------------|-----------:|----------:|----------:|-----------:|-----------:|
| 📍 **StaticEcsContext** | 100000      |   1.681 ms | 0.0074 ms | 0.0058 ms |   1.681 ms |      400 B |
| DragonECSContext        | 100000      |   2.911 ms | 0.0233 ms | 0.0195 ms |   2.908 ms |      400 B |
| DefaultECSContext       | 100000      |   2.945 ms | 0.0509 ms | 0.0993 ms |   2.980 ms |      400 B |
| MassiveEcsContext       | 100000      |   3.029 ms | 0.1381 ms | 0.3803 ms |   2.865 ms |      400 B |
| FrifloContext           | 100000      |   3.543 ms | 0.0704 ms | 0.0811 ms |   3.559 ms |      400 B |
| LeoEcsLiteContext       | 100000      |   4.109 ms | 0.0853 ms | 0.2276 ms |   4.047 ms |      400 B |
| MorpehContext           | 100000      |   5.028 ms | 0.0922 ms | 0.0817 ms |   4.990 ms |      400 B |
| TinyEcsContext          | 100000      |   5.376 ms | 0.1007 ms | 0.0892 ms |   5.366 ms |  2400400 B |
| ArchContext             | 100000      |   5.663 ms | 0.0472 ms | 0.0442 ms |   5.648 ms |     1408 B |
| XenoContext             | 100000      |   6.089 ms | 0.0853 ms | 0.0712 ms |   6.083 ms |   787632 B |
| LeoEcsContext           | 100000      |   6.262 ms | 0.0881 ms | 0.1566 ms |   6.258 ms |      400 B |
| FennecsContext          | 100000      |  33.970 ms | 0.5286 ms | 0.4945 ms |  33.887 ms | 49600400 B |
| FlecsNETContext         | 100000      | 111.751 ms | 2.0706 ms | 1.7290 ms | 112.060 ms |      400 B |

# Add2ComponentsRandomOrder

| Context                 | EntityCount |       Mean |     Error |    StdDev |   Allocated |
|-------------------------|-------------|-----------:|----------:|----------:|------------:|
| 📍 **StaticEcsContext** | 100000      |   1.470 ms | 0.0430 ms | 0.1190 ms |       400 B |
| MassiveEcsContext       | 100000      |   2.099 ms | 0.0454 ms | 0.1173 ms |       400 B |
| FrifloContext           | 100000      |   6.489 ms | 0.3175 ms | 0.9111 ms |       400 B |
| DragonECSContext        | 100000      |   6.504 ms | 0.1267 ms | 0.3382 ms |       400 B |
| LeoEcsLiteContext       | 100000      |   8.423 ms | 0.4666 ms | 1.3685 ms |       112 B |
| XenoContext             | 100000      |   9.828 ms | 0.4829 ms | 1.3855 ms |    983688 B |
| ArchContext             | 100000      |  10.108 ms | 0.3215 ms | 0.9069 ms |      1456 B |
| DefaultECSContext       | 100000      |  16.253 ms | 0.4367 ms | 1.2807 ms |       400 B |
| TinyEcsContext          | 100000      |  16.811 ms | 0.3341 ms | 0.8195 ms |   4800400 B |
| LeoEcsContext           | 100000      |  19.954 ms | 0.3923 ms | 0.7272 ms |       400 B |
| MorpehContext           | 100000      |  23.750 ms | 0.4696 ms | 0.7029 ms |       400 B |
| FennecsContext          | 100000      |  91.486 ms | 0.8186 ms | 0.7657 ms | 113600400 B |
| FlecsNETContext         | 100000      | 197.377 ms | 1.2173 ms | 1.0791 ms |       400 B |

# Add2Components

| Context                 | EntityCount |         Mean |       Error |      StdDev |   Allocated |
|-------------------------|-------------|-------------:|------------:|------------:|------------:|
| 📍 **StaticEcsContext** | 100000      |     711.2 μs |     3.38 μs |     2.99 μs |       400 B |
| DragonECSContext        | 100000      |   1,330.1 μs |    24.58 μs |    26.30 μs |       400 B |
| MassiveEcsContext       | 100000      |   1,418.7 μs |    15.91 μs |    27.01 μs |       400 B |
| DefaultECSContext       | 100000      |   2,733.2 μs |    38.92 μs |    32.50 μs |       400 B |
| MorpehContext           | 100000      |   3,116.8 μs |    38.81 μs |    30.30 μs |        64 B |
| LeoEcsLiteContext       | 100000      |   3,291.2 μs |    29.45 μs |    26.11 μs |       400 B |
| FrifloContext           | 100000      |   3,436.8 μs |    48.65 μs |    37.98 μs |       400 B |
| XenoContext             | 100000      |   3,589.3 μs |    70.39 μs |    78.24 μs |    983688 B |
| LeoEcsContext           | 100000      |   5,511.7 μs |    99.10 μs |   132.29 μs |       400 B |
| ArchContext             | 100000      |   6,345.3 μs |    64.92 μs |    57.55 μs |      1744 B |
| TinyEcsContext          | 100000      |   7,375.1 μs |   140.14 μs |   143.92 μs |   4800400 B |
| FennecsContext          | 100000      |  72,436.8 μs |   621.80 μs |   551.21 μs | 113600400 B |
| FlecsNETContext         | 100000      | 162,026.3 μs | 2,829.77 μs | 2,362.98 μs |       400 B |

# Add2RandomComponentsRandomOrder

| Context                 | EntityCount |       Mean |     Error |    StdDev |     Median |   Allocated |
|-------------------------|-------------|-----------:|----------:|----------:|-----------:|------------:|
| 📍 **StaticEcsContext** | 100000      |   3.420 ms | 0.1930 ms | 0.5600 ms |   3.261 ms |       400 B |
| MassiveEcsContext       | 100000      |   5.611 ms | 0.2149 ms | 0.5882 ms |   5.537 ms |       400 B |
| FrifloContext           | 100000      |   9.188 ms | 0.3591 ms | 1.0303 ms |   9.051 ms |       400 B |
| DragonECSContext        | 100000      |  12.239 ms | 0.2442 ms | 0.4526 ms |  12.139 ms |       400 B |
| ArchContext             | 100000      |  12.694 ms | 0.2515 ms | 0.4965 ms |  12.571 ms |      1744 B |
| XenoContext             | 100000      |  15.490 ms | 0.4930 ms | 1.4146 ms |  15.168 ms |    787632 B |
| TinyEcsContext          | 100000      |  17.946 ms | 0.3540 ms | 0.2956 ms |  17.924 ms |   4800400 B |
| LeoEcsLiteContext       | 100000      |  18.371 ms | 0.3790 ms | 1.0873 ms |  18.298 ms |       400 B |
| DefaultECSContext       | 100000      |  18.813 ms | 0.3735 ms | 0.8582 ms |  18.614 ms |       400 B |
| MorpehContext           | 100000      |  24.813 ms | 0.4892 ms | 0.5234 ms |  24.805 ms |       400 B |
| LeoEcsContext           | 100000      |  28.571 ms | 0.7012 ms | 2.0231 ms |  27.940 ms |       400 B |
| FennecsContext          | 100000      |  96.431 ms | 0.9178 ms | 0.7664 ms |  96.510 ms | 113600400 B |
| FlecsNETContext         | 100000      | 237.303 ms | 4.3926 ms | 4.1088 ms | 234.932 ms |       400 B |

# Add2RandomComponents

| Context                 | EntityCount |       Mean |     Error |    StdDev |     Median |   Allocated |
|-------------------------|-------------|-----------:|----------:|----------:|-----------:|------------:|
| 📍 **StaticEcsContext** | 100000      |   2.043 ms | 0.0174 ms | 0.0163 ms |   2.037 ms |       400 B |
| DragonECSContext        | 100000      |   3.482 ms | 0.0246 ms | 0.0206 ms |   3.481 ms |       400 B |
| DefaultECSContext       | 100000      |   4.285 ms | 0.0524 ms | 0.0437 ms |   4.263 ms |       400 B |
| MassiveEcsContext       | 100000      |   4.891 ms | 0.5860 ms | 1.7187 ms |   3.776 ms |       400 B |
| FrifloContext           | 100000      |   5.377 ms | 0.0689 ms | 0.0846 ms |   5.366 ms |       400 B |
| MorpehContext           | 100000      |   6.108 ms | 0.1011 ms | 0.0790 ms |   6.078 ms |       400 B |
| LeoEcsLiteContext       | 100000      |   7.293 ms | 0.0733 ms | 0.0572 ms |   7.302 ms |       400 B |
| XenoContext             | 100000      |   7.307 ms | 0.0883 ms | 0.0738 ms |   7.275 ms |    787632 B |
| ArchContext             | 100000      |   8.219 ms | 0.1224 ms | 0.1022 ms |   8.230 ms |      1744 B |
| TinyEcsContext          | 100000      |   8.567 ms | 0.1657 ms | 0.1384 ms |   8.538 ms |   4800400 B |
| LeoEcsContext           | 100000      |   9.701 ms | 0.1925 ms | 0.2291 ms |   9.733 ms |       400 B |
| FennecsContext          | 100000      |  77.875 ms | 1.3273 ms | 2.4270 ms |  77.489 ms | 113600400 B |
| FlecsNETContext         | 100000      | 210.936 ms | 0.9781 ms | 0.9149 ms | 210.695 ms |       400 B |

# Add3ComponentsRandomOrder

| Context                 | EntityCount |       Mean |     Error |    StdDev |     Median |   Allocated |
|-------------------------|-------------|-----------:|----------:|----------:|-----------:|------------:|
| 📍 **StaticEcsContext** | 100000      |   2.192 ms | 0.1100 ms | 0.3138 ms |   2.094 ms |       400 B |
| MassiveEcsContext       | 100000      |   3.441 ms | 0.0808 ms | 0.2358 ms |   3.400 ms |       400 B |
| FrifloContext           | 100000      |   9.014 ms | 0.3626 ms | 1.0287 ms |   9.059 ms |       400 B |
| ArchContext             | 100000      |  10.741 ms | 0.2999 ms | 0.8509 ms |  10.627 ms |      1744 B |
| DragonECSContext        | 100000      |  10.743 ms | 0.2121 ms | 0.4136 ms |  10.640 ms |       400 B |
| LeoEcsLiteContext       | 100000      |  13.422 ms | 0.2643 ms | 0.7539 ms |  13.319 ms |       400 B |
| XenoContext             | 100000      |  13.652 ms | 0.2894 ms | 0.7674 ms |  13.647 ms |    983696 B |
| DefaultECSContext       | 100000      |  19.222 ms | 0.3802 ms | 1.0660 ms |  18.979 ms |       400 B |
| LeoEcsContext           | 100000      |  23.449 ms | 0.4359 ms | 0.8604 ms |  23.113 ms |       400 B |
| MorpehContext           | 100000      |  25.740 ms | 0.4923 ms | 0.6226 ms |  25.586 ms |       400 B |
| TinyEcsContext          | 100000      |  35.430 ms | 1.9097 ms | 5.6307 ms |  32.426 ms |  57532304 B |
| FennecsContext          | 100000      | 144.353 ms | 1.3627 ms | 1.2080 ms | 144.044 ms | 184800400 B |
| FlecsNETContext         | 100000      | 273.425 ms | 2.3467 ms | 2.0803 ms | 273.875 ms |       400 B |

# Add3Components

| Context                 | EntityCount |       Mean |     Error |    StdDev |     Median |   Allocated |
|-------------------------|-------------|-----------:|----------:|----------:|-----------:|------------:|
| 📍 **StaticEcsContext** | 100000      |   1.062 ms | 0.0097 ms | 0.0081 ms |   1.060 ms |       400 B |
| DragonECSContext        | 100000      |   1.963 ms | 0.0360 ms | 0.0539 ms |   1.957 ms |       400 B |
| MassiveEcsContext       | 100000      |   2.135 ms | 0.0504 ms | 0.1446 ms |   2.060 ms |       400 B |
| MorpehContext           | 100000      |   3.996 ms | 0.0795 ms | 0.0946 ms |   4.008 ms |        64 B |
| DefaultECSContext       | 100000      |   4.120 ms | 0.0817 ms | 0.0908 ms |   4.086 ms |       400 B |
| XenoContext             | 100000      |   4.223 ms | 0.0515 ms | 0.0402 ms |   4.224 ms |    983696 B |
| FrifloContext           | 100000      |   5.777 ms | 0.0964 ms | 0.1472 ms |   5.729 ms |       400 B |
| LeoEcsLiteContext       | 100000      |   6.110 ms | 0.0959 ms | 0.0850 ms |   6.115 ms |       112 B |
| ArchContext             | 100000      |   6.996 ms | 0.1194 ms | 0.1058 ms |   6.971 ms |      1408 B |
| LeoEcsContext           | 100000      |   8.011 ms | 0.1488 ms | 0.2759 ms |   7.924 ms |       400 B |
| TinyEcsContext          | 100000      |  25.264 ms | 2.0780 ms | 6.0944 ms |  21.871 ms |  57532304 B |
| FennecsContext          | 100000      | 121.252 ms | 1.7178 ms | 1.5228 ms | 120.924 ms | 184800400 B |
| FlecsNETContext         | 100000      | 232.786 ms | 2.0729 ms | 1.9390 ms | 232.328 ms |       400 B |

# Add3RandomComponentsRandomOrder

| Context                 | EntityCount |       Mean |     Error |    StdDev |     Median |   Allocated |
|-------------------------|-------------|-----------:|----------:|----------:|-----------:|------------:|
| 📍 **StaticEcsContext** | 100000      |   4.743 ms | 0.4582 ms | 1.3510 ms |   4.152 ms |       400 B |
| MassiveEcsContext       | 100000      |   6.951 ms | 0.2198 ms | 0.5943 ms |   6.876 ms |       400 B |
| FrifloContext           | 100000      |  13.109 ms | 0.3991 ms | 1.1516 ms |  12.951 ms |       400 B |
| DragonECSContext        | 100000      |  14.786 ms | 0.2942 ms | 0.6458 ms |  14.652 ms |       400 B |
| ArchContext             | 100000      |  16.129 ms | 0.7696 ms | 2.2693 ms |  15.551 ms |      1744 B |
| XenoContext             | 100000      |  19.565 ms | 0.4771 ms | 1.3536 ms |  19.416 ms |    787664 B |
| DefaultECSContext       | 100000      |  23.399 ms | 0.4674 ms | 1.2796 ms |  23.149 ms |       400 B |
| LeoEcsLiteContext       | 100000      |  26.179 ms | 0.5207 ms | 1.2476 ms |  26.057 ms |       400 B |
| MorpehContext           | 100000      |  30.343 ms | 0.5824 ms | 1.5445 ms |  30.083 ms |       400 B |
| LeoEcsContext           | 100000      |  34.458 ms | 0.6171 ms | 0.9237 ms |  34.424 ms |       400 B |
| TinyEcsContext          | 100000      |  37.687 ms | 2.3413 ms | 6.8296 ms |  34.254 ms |  57532304 B |
| FennecsContext          | 100000      | 151.198 ms | 2.9625 ms | 2.6262 ms | 150.506 ms | 184800400 B |
| FlecsNETContext         | 100000      | 310.308 ms | 2.1757 ms | 1.9287 ms | 310.102 ms |       400 B |

# Add3RandomComponents

| Context                 | EntityCount |       Mean |     Error |    StdDev |     Median |   Allocated |
|-------------------------|-------------|-----------:|----------:|----------:|-----------:|------------:|
| 📍 **StaticEcsContext** | 100000      |   2.336 ms | 0.0051 ms | 0.0043 ms |   2.337 ms |       400 B |
| DragonECSContext        | 100000      |   4.167 ms | 0.0582 ms | 0.0647 ms |   4.159 ms |       400 B |
| MassiveEcsContext       | 100000      |   4.829 ms | 0.0406 ms | 0.0783 ms |   4.812 ms |       400 B |
| DefaultECSContext       | 100000      |   5.599 ms | 0.1095 ms | 0.1076 ms |   5.555 ms |       400 B |
| MorpehContext           | 100000      |   6.853 ms | 0.0474 ms | 0.0370 ms |   6.848 ms |       400 B |
| XenoContext             | 100000      |   7.641 ms | 0.1381 ms | 0.3414 ms |   7.543 ms |    787664 B |
| ArchContext             | 100000      |   8.376 ms | 0.1607 ms | 0.1579 ms |   8.322 ms |      1744 B |
| FrifloContext           | 100000      |   8.423 ms | 0.0738 ms | 0.0616 ms |   8.418 ms |       400 B |
| LeoEcsLiteContext       | 100000      |  11.864 ms | 0.0739 ms | 0.0577 ms |  11.864 ms |       400 B |
| LeoEcsContext           | 100000      |  15.542 ms | 0.2987 ms | 0.2794 ms |  15.617 ms |       112 B |
| TinyEcsContext          | 100000      |  26.790 ms | 2.0935 ms | 6.1398 ms |  23.056 ms |  57531968 B |
| FennecsContext          | 100000      | 128.606 ms | 1.8891 ms | 1.7671 ms | 127.892 ms | 184800400 B |
| FlecsNETContext         | 100000      | 284.572 ms | 1.6003 ms | 1.4187 ms | 284.450 ms |       400 B |

# Add4ComponentsRandomOrder

| Context                 | EntityCount |       Mean |     Error |    StdDev |     Median |   Allocated |
|-------------------------|-------------|-----------:|----------:|----------:|-----------:|------------:|
| 📍 **StaticEcsContext** | 100000      |   3.078 ms | 0.1780 ms | 0.5106 ms |   2.938 ms |       400 B |
| MassiveEcsContext       | 100000      |   5.368 ms | 0.3726 ms | 1.0508 ms |   4.888 ms |       112 B |
| FrifloContext           | 100000      |  12.377 ms | 0.2889 ms | 0.8242 ms |  12.205 ms |       400 B |
| DragonECSContext        | 100000      |  12.743 ms | 0.2539 ms | 0.6035 ms |  12.653 ms |       400 B |
| ArchContext             | 100000      |  13.654 ms | 0.8473 ms | 2.4851 ms |  13.370 ms |      1120 B |
| XenoContext             | 100000      |  15.968 ms | 0.6115 ms | 1.7047 ms |  15.596 ms |    983696 B |
| LeoEcsLiteContext       | 100000      |  21.945 ms | 1.5506 ms | 4.4240 ms |  20.902 ms |       400 B |
| DefaultECSContext       | 100000      |  24.115 ms | 0.9139 ms | 2.6223 ms |  23.068 ms |       400 B |
| LeoEcsContext           | 100000      |  27.156 ms | 0.5210 ms | 0.4873 ms |  27.220 ms |       400 B |
| MorpehContext           | 100000      |  31.457 ms | 0.7378 ms | 2.1051 ms |  31.176 ms |       400 B |
| TinyEcsContext          | 100000      |  42.391 ms | 2.4545 ms | 7.1598 ms |  40.093 ms |   9600400 B |
| FennecsContext          | 100000      | 203.985 ms | 2.0196 ms | 1.6864 ms | 203.814 ms | 268800400 B |
| FlecsNETContext         | 100000      | 332.370 ms | 1.8946 ms | 1.7722 ms | 332.526 ms |       400 B |

# Add4Components

| Context                 | EntityCount |       Mean |     Error |    StdDev |     Median |   Allocated |
|-------------------------|-------------|-----------:|----------:|----------:|-----------:|------------:|
| 📍 **StaticEcsContext** | 100000      |   1.420 ms | 0.0151 ms | 0.0127 ms |   1.424 ms |       400 B |
| DragonECSContext        | 100000      |   2.545 ms | 0.0455 ms | 0.0403 ms |   2.540 ms |       400 B |
| MassiveEcsContext       | 100000      |   2.694 ms | 0.0093 ms | 0.0072 ms |   2.696 ms |       400 B |
| MorpehContext           | 100000      |   4.824 ms | 0.0516 ms | 0.0483 ms |   4.824 ms |       400 B |
| XenoContext             | 100000      |   4.893 ms | 0.0957 ms | 0.0940 ms |   4.862 ms |    983696 B |
| DefaultECSContext       | 100000      |   5.437 ms | 0.0945 ms | 0.0838 ms |   5.457 ms |       400 B |
| ArchContext             | 100000      |   7.002 ms | 0.0462 ms | 0.0386 ms |   7.004 ms |      1744 B |
| FrifloContext           | 100000      |   8.077 ms | 0.1532 ms | 0.2429 ms |   7.977 ms |       400 B |
| LeoEcsLiteContext       | 100000      |   8.451 ms | 0.0515 ms | 0.0430 ms |   8.449 ms |       400 B |
| LeoEcsContext           | 100000      |  12.157 ms | 0.1204 ms | 0.1067 ms |  12.129 ms |       112 B |
| TinyEcsContext          | 100000      |  30.183 ms | 2.4580 ms | 7.2476 ms |  25.107 ms |   9600112 B |
| FennecsContext          | 100000      | 185.914 ms | 3.6717 ms | 6.4307 ms | 184.143 ms | 268800064 B |
| FlecsNETContext         | 100000      | 294.637 ms | 1.8993 ms | 1.7766 ms | 294.268 ms |       400 B |

# CreateEmptyEntity

| Context                 | EntityCount |       Mean |     Error |    StdDev |     Median | Allocated |
|-------------------------|-------------|-----------:|----------:|----------:|-----------:|----------:|
| MorpehContext           | 100000      |   290.2 μs |   5.25 μs |   9.59 μs |   289.4 μs |     400 B |
| 📍 **StaticEcsContext** | 100000      |   298.5 μs |   3.01 μs |   2.51 μs |   298.0 μs |     112 B |
| MassiveEcsContext       | 100000      |   317.4 μs |   4.79 μs |   3.74 μs |   317.5 μs |     400 B |
| LeoEcsContext           | 100000      |   441.3 μs |   8.78 μs |  13.14 μs |   439.1 μs |      64 B |
| DragonECSContext        | 100000      |   700.0 μs |  29.24 μs |  79.55 μs |   672.3 μs |      64 B |
| LeoEcsLiteContext       | 100000      |   916.8 μs |   5.85 μs |   4.89 μs |   916.1 μs |     400 B |
| FrifloContext           | 100000      | 1,157.4 μs | 111.67 μs | 323.98 μs | 1,254.0 μs |      64 B |
| DefaultECSContext       | 100000      | 1,292.9 μs |  16.85 μs |  25.74 μs | 1,290.5 μs |     400 B |
| XenoContext             | 100000      | 1,810.7 μs |  36.13 μs |  91.31 μs | 1,779.7 μs |     112 B |
| FennecsContext          | 100000      | 1,915.4 μs |  33.74 μs |  76.15 μs | 1,900.2 μs |     112 B |
| TinyEcsContext          | 100000      | 4,453.2 μs | 118.89 μs | 331.41 μs | 4,424.4 μs | 6146800 B |
| ArchContext             | 100000      | 4,631.1 μs |  91.13 μs | 154.74 μs | 4,636.6 μs |     736 B |
| FlecsNETContext         | 100000      | 8,123.4 μs | 159.17 μs | 261.52 μs | 8,165.1 μs |     400 B |

# CreateEntityWith1Component

| Context                 | EntityCount |        Mean |       Error |      StdDev |      Median |  Allocated |
|-------------------------|-------------|------------:|------------:|------------:|------------:|-----------:|
| 📍 **StaticEcsContext** | 100000      |    658.0 μs |    24.77 μs |    66.95 μs |    626.9 μs |      400 B |
| DragonECSContext        | 100000      |  1,804.3 μs |    35.78 μs |    47.76 μs |  1,793.8 μs |      400 B |
| LeoEcsLiteContext       | 100000      |  2,086.6 μs |    39.13 μs |   103.76 μs |  2,073.6 μs |      400 B |
| MassiveEcsContext       | 100000      |  2,110.5 μs |    24.78 μs |    19.35 μs |  2,107.8 μs |      400 B |
| FrifloContext           | 100000      |  2,516.5 μs |   227.58 μs |   603.51 μs |  2,123.1 μs |      400 B |
| XenoContext             | 100000      |  2,586.1 μs |    50.32 μs |    63.64 μs |  2,579.8 μs |   983624 B |
| MorpehContext           | 100000      |  2,944.6 μs |    74.84 μs |   201.06 μs |  2,897.1 μs |      400 B |
| DefaultECSContext       | 100000      |  3,133.7 μs |    54.17 μs |    89.00 μs |  3,102.8 μs |       64 B |
| LeoEcsContext           | 100000      |  3,996.9 μs |    79.59 μs |   216.54 μs |  3,969.9 μs |      400 B |
| ArchContext             | 100000      |  4,181.2 μs |   310.66 μs |   915.98 μs |  3,714.2 μs |     1072 B |
| TinyEcsContext          | 100000      |  8,012.0 μs |   240.02 μs |   657.06 μs |  7,910.9 μs |  8300944 B |
| FennecsContext          | 100000      | 37,376.4 μs |   736.74 μs | 1,722.10 μs | 37,001.5 μs | 49600064 B |
| FlecsNETContext         | 100000      | 78,954.4 μs | 1,554.19 μs | 2,020.89 μs | 79,281.4 μs |      112 B |

# CreateEntityWith1RandomComponent

| Context                 | EntityCount | ChunkSize |       Mean |     Error |    StdDev |     Median |   Allocated |
|-------------------------|-------------|-----------|-----------:|----------:|----------:|-----------:|------------:|
| 📍 **StaticEcsContext** | 100000      | 1         |   2.593 ms | 0.0508 ms | 0.0605 ms |   2.600 ms |       400 B |
| FrifloContext           | 100000      | 1         |   4.564 ms | 0.1037 ms | 0.2804 ms |   4.647 ms |       400 B |
| XenoContext             | 100000      | 1         |   4.667 ms | 0.0927 ms | 0.1139 ms |   4.660 ms |       400 B |
| MassiveEcsContext       | 100000      | 1         |   4.887 ms | 0.0393 ms | 0.0307 ms |   4.896 ms |    527344 B |
| DragonECSContext        | 100000      | 1         |   5.791 ms | 0.1416 ms | 0.3779 ms |   5.734 ms |       112 B |
| LeoEcsLiteContext       | 100000      | 1         |   6.136 ms | 0.3673 ms | 0.9481 ms |   5.890 ms |       400 B |
| DefaultECSContext       | 100000      | 1         |   6.342 ms | 0.1260 ms | 0.2272 ms |   6.278 ms |   3200112 B |
| MorpehContext           | 100000      | 1         |   7.059 ms | 0.1827 ms | 0.5122 ms |   6.985 ms |       400 B |
| ArchContext             | 100000      | 1         |   8.486 ms | 0.2978 ms | 0.8447 ms |   8.638 ms |  19488912 B |
| TinyEcsContext          | 100000      | 1         |  16.153 ms | 1.2165 ms | 3.2682 ms |  17.967 ms |   8481216 B |
| LeoEcsContext           | 100000      | 1         |  17.019 ms | 0.3397 ms | 0.8396 ms |  17.074 ms |   8800112 B |
| FennecsContext          | 100000      | 1         |  39.963 ms | 0.7964 ms | 1.8927 ms |  39.439 ms | 653580392 B |
| FlecsNETContext         | 100000      | 1         | 134.562 ms | 2.6479 ms | 2.9432 ms | 134.608 ms |       400 B |
|                         |             |           |            |           |           |            |             |
| 📍 **StaticEcsContext** | 100000      | 4         |   1.316 ms | 0.0260 ms | 0.0549 ms |   1.293 ms |       400 B |
| XenoContext             | 100000      | 4         |   2.640 ms | 0.0522 ms | 0.1250 ms |   2.597 ms |       400 B |
| FrifloContext           | 100000      | 4         |   2.956 ms | 0.0908 ms | 0.2455 ms |   2.834 ms |       400 B |
| MassiveEcsContext       | 100000      | 4         |   3.077 ms | 0.3002 ms | 0.8466 ms |   3.093 ms |    401984 B |
| DragonECSContext        | 100000      | 4         |   4.181 ms | 0.1682 ms | 0.4718 ms |   4.036 ms |       400 B |
| MorpehContext           | 100000      | 4         |   4.504 ms | 0.1374 ms | 0.3785 ms |   4.452 ms |       400 B |
| DefaultECSContext       | 100000      | 4         |   4.990 ms | 0.0969 ms | 0.2126 ms |   4.950 ms |   3200400 B |
| ArchContext             | 100000      | 4         |   7.359 ms | 0.2800 ms | 0.8169 ms |   7.407 ms |  20201488 B |
| LeoEcsLiteContext       | 100000      | 4         |   9.532 ms | 0.1563 ms | 0.1305 ms |   9.580 ms |       400 B |
| TinyEcsContext          | 100000      | 4         |  10.219 ms | 0.2878 ms | 0.7632 ms |  10.421 ms |   8480928 B |
| LeoEcsContext           | 100000      | 4         |  13.870 ms | 0.2682 ms | 0.4971 ms |  13.843 ms |   8800400 B |
| FennecsContext          | 100000      | 4         |  36.919 ms | 0.7359 ms | 0.8475 ms |  36.886 ms |  49600400 B |
| FlecsNETContext         | 100000      | 4         |  98.096 ms | 1.9552 ms | 1.8289 ms |  97.974 ms |       400 B |
|                         |             |           |            |           |           |            |             |
| 📍 **StaticEcsContext** | 100000      | 32        |   1.086 ms | 0.0217 ms | 0.0595 ms |   1.094 ms |       400 B |
| XenoContext             | 100000      | 32        |   2.112 ms | 0.0424 ms | 0.1166 ms |   2.088 ms |       400 B |
| MassiveEcsContext       | 100000      | 32        |   2.313 ms | 0.2474 ms | 0.6896 ms |   1.926 ms |    401896 B |
| FrifloContext           | 100000      | 32        |   2.437 ms | 0.0493 ms | 0.1315 ms |   2.374 ms |       400 B |
| MorpehContext           | 100000      | 32        |   3.319 ms | 0.0660 ms | 0.1643 ms |   3.285 ms |       400 B |
| DragonECSContext        | 100000      | 32        |   3.953 ms | 0.1510 ms | 0.4183 ms |   3.928 ms |       400 B |
| DefaultECSContext       | 100000      | 32        |   4.602 ms | 0.0920 ms | 0.1795 ms |   4.568 ms |   3200112 B |
| LeoEcsLiteContext       | 100000      | 32        |   5.406 ms | 1.1291 ms | 3.2576 ms |   3.566 ms |       400 B |
| ArchContext             | 100000      | 32        |   7.129 ms | 0.2950 ms | 0.8273 ms |   6.995 ms |  20092736 B |
| TinyEcsContext          | 100000      | 32        |   8.675 ms | 0.2644 ms | 0.6826 ms |   8.533 ms |   8480928 B |
| LeoEcsContext           | 100000      | 32        |  12.111 ms | 0.2550 ms | 0.6674 ms |  11.966 ms |   8800400 B |
| FennecsContext          | 100000      | 32        |  35.400 ms | 0.7037 ms | 1.1161 ms |  35.116 ms |  49600064 B |
| FlecsNETContext         | 100000      | 32        |  87.804 ms | 1.6961 ms | 2.3217 ms |  87.159 ms |       400 B |

# CreateEntityWith2Components

| Context                 | EntityCount |       Mean |     Error |    StdDev |     Median |   Allocated |
|-------------------------|-------------|-----------:|----------:|----------:|-----------:|------------:|
| 📍 **StaticEcsContext** | 100000      |   1.103 ms | 0.0626 ms | 0.1765 ms |   1.014 ms |       112 B |
| MassiveEcsContext       | 100000      |   1.916 ms | 0.1342 ms | 0.3719 ms |   1.694 ms |       400 B |
| DragonECSContext        | 100000      |   2.359 ms | 0.0668 ms | 0.1929 ms |   2.268 ms |       400 B |
| XenoContext             | 100000      |   2.520 ms | 0.1241 ms | 0.3560 ms |   2.360 ms |    983624 B |
| ArchContext             | 100000      |   3.359 ms | 0.0260 ms | 0.0230 ms |   3.360 ms |       784 B |
| MorpehContext           | 100000      |   3.511 ms | 0.0687 ms | 0.0764 ms |   3.507 ms |       400 B |
| LeoEcsLiteContext       | 100000      |   3.763 ms | 0.0865 ms | 0.2522 ms |   3.663 ms |       400 B |
| FrifloContext           | 100000      |   4.210 ms | 0.0829 ms | 0.1162 ms |   4.163 ms |       400 B |
| DefaultECSContext       | 100000      |   4.363 ms | 0.0282 ms | 0.0250 ms |   4.364 ms |       400 B |
| LeoEcsContext           | 100000      |   6.206 ms | 0.1221 ms | 0.2494 ms |   6.112 ms |       400 B |
| TinyEcsContext          | 100000      |  24.354 ms | 0.4325 ms | 0.3377 ms |  24.374 ms |  10946800 B |
| FennecsContext          | 100000      |  76.463 ms | 1.2396 ms | 1.0989 ms |  76.447 ms | 113600400 B |
| FlecsNETContext         | 100000      | 176.070 ms | 2.3039 ms | 1.9239 ms | 176.356 ms |       400 B |

# CreateEntityWith2RandomComponents

| Context                 | EntityCount | ChunkSize |       Mean |     Error |    StdDev |     Median |   Allocated |
|-------------------------|-------------|-----------|-----------:|----------:|----------:|-----------:|------------:|
| 📍 **StaticEcsContext** | 100000      | 1         |   3.200 ms | 0.0614 ms | 0.1596 ms |   3.194 ms |       400 B |
| MassiveEcsContext       | 100000      | 1         |   5.484 ms | 0.1060 ms | 0.2639 ms |   5.468 ms |    818128 B |
| XenoContext             | 100000      | 1         |   5.824 ms | 0.1131 ms | 0.2941 ms |   5.720 ms |       400 B |
| DragonECSContext        | 100000      | 1         |   6.494 ms | 0.1285 ms | 0.3104 ms |   6.485 ms |       400 B |
| FrifloContext           | 100000      | 1         |   6.616 ms | 0.0714 ms | 0.1153 ms |   6.660 ms |       400 B |
| DefaultECSContext       | 100000      | 1         |   7.662 ms | 0.1469 ms | 0.1508 ms |   7.668 ms |   3200400 B |
| MorpehContext           | 100000      | 1         |   8.101 ms | 0.1595 ms | 0.2621 ms |   8.043 ms |       112 B |
| ArchContext             | 100000      | 1         |   8.780 ms | 0.2207 ms | 0.6261 ms |   8.847 ms |  20243008 B |
| LeoEcsLiteContext       | 100000      | 1         |   9.069 ms | 0.1798 ms | 0.3909 ms |   8.910 ms |       400 B |
| LeoEcsContext           | 100000      | 1         |  21.839 ms | 0.4366 ms | 0.8720 ms |  21.805 ms |   8800400 B |
| TinyEcsContext          | 100000      | 1         |  29.947 ms | 0.8985 ms | 2.3193 ms |  31.130 ms |  10880928 B |
| FennecsContext          | 100000      | 1         |  79.981 ms | 1.5655 ms | 2.1946 ms |  79.826 ms | 113600400 B |
| FlecsNETContext         | 100000      | 1         | 235.320 ms | 3.1086 ms | 2.7557 ms | 234.435 ms |       400 B |
|                         |             |           |            |           |           |            |             |
| 📍 **StaticEcsContext** | 100000      | 4         |   1.878 ms | 0.0372 ms | 0.0831 ms |   1.860 ms |       400 B |
| XenoContext             | 100000      | 4         |   3.387 ms | 0.0662 ms | 0.0837 ms |   3.387 ms |       400 B |
| DragonECSContext        | 100000      | 4         |   4.829 ms | 0.1477 ms | 0.4117 ms |   4.668 ms |       400 B |
| MassiveEcsContext       | 100000      | 4         |   5.153 ms | 1.0126 ms | 2.8560 ms |   3.766 ms |    818992 B |
| FrifloContext           | 100000      | 4         |   5.271 ms | 0.0938 ms | 0.1042 ms |   5.241 ms |       400 B |
| MorpehContext           | 100000      | 4         |   5.331 ms | 0.1184 ms | 0.3241 ms |   5.265 ms |       112 B |
| DefaultECSContext       | 100000      | 4         |   6.319 ms | 0.1229 ms | 0.1682 ms |   6.302 ms |   3200400 B |
| ArchContext             | 100000      | 4         |   7.098 ms | 0.2424 ms | 0.7033 ms |   7.175 ms |  20925344 B |
| LeoEcsLiteContext       | 100000      | 4         |   7.465 ms | 0.1488 ms | 0.3840 ms |   7.327 ms |       400 B |
| LeoEcsContext           | 100000      | 4         |  18.063 ms | 0.3467 ms | 0.6071 ms |  18.005 ms |   8800400 B |
| TinyEcsContext          | 100000      | 4         |  27.273 ms | 1.7803 ms | 5.1365 ms |  24.966 ms |  10881216 B |
| FennecsContext          | 100000      | 4         |  80.944 ms | 1.5566 ms | 1.2999 ms |  81.027 ms | 113600400 B |
| FlecsNETContext         | 100000      | 4         | 198.212 ms | 2.8209 ms | 2.3556 ms | 197.363 ms |       400 B |
|                         |             |           |            |           |           |            |             |
| 📍 **StaticEcsContext** | 100000      | 32        |   1.583 ms | 0.0325 ms | 0.0891 ms |   1.572 ms |       400 B |
| XenoContext             | 100000      | 32        |   2.984 ms | 0.0702 ms | 0.1934 ms |   2.979 ms |       400 B |
| FrifloContext           | 100000      | 32        |   4.294 ms | 0.0694 ms | 0.1336 ms |   4.268 ms |       400 B |
| MorpehContext           | 100000      | 32        |   4.360 ms | 0.0806 ms | 0.1231 ms |   4.344 ms |       400 B |
| MassiveEcsContext       | 100000      | 32        |   4.431 ms | 0.9323 ms | 2.6297 ms |   3.207 ms |    887528 B |
| DragonECSContext        | 100000      | 32        |   4.466 ms | 0.1424 ms | 0.3946 ms |   4.392 ms |       112 B |
| DefaultECSContext       | 100000      | 32        |   6.098 ms | 0.1213 ms | 0.3088 ms |   6.031 ms |   3200400 B |
| LeoEcsLiteContext       | 100000      | 32        |   6.831 ms | 0.1280 ms | 0.3373 ms |   6.723 ms |       400 B |
| ArchContext             | 100000      | 32        |   6.956 ms | 0.2286 ms | 0.6704 ms |   7.021 ms |  21064072 B |
| LeoEcsContext           | 100000      | 32        |  16.118 ms | 0.2497 ms | 0.5044 ms |  16.045 ms |   8800112 B |
| TinyEcsContext          | 100000      | 32        |  26.701 ms | 1.7982 ms | 5.2738 ms |  23.591 ms |  10881216 B |
| FennecsContext          | 100000      | 32        |  79.005 ms | 1.5028 ms | 1.3322 ms |  79.137 ms | 113600400 B |
| FlecsNETContext         | 100000      | 32        | 190.383 ms | 3.5242 ms | 3.2965 ms | 189.939 ms |       400 B |

# CreateEntityWith3Components

| Context                 | EntityCount |       Mean |     Error |    StdDev |     Median |   Allocated |
|-------------------------|-------------|-----------:|----------:|----------:|-----------:|------------:|
| 📍 **StaticEcsContext** | 100000      |   1.380 ms | 0.0119 ms | 0.0100 ms |   1.377 ms |       400 B |
| MassiveEcsContext       | 100000      |   2.432 ms | 0.0448 ms | 0.0598 ms |   2.416 ms |       400 B |
| DragonECSContext        | 100000      |   2.956 ms | 0.0527 ms | 0.0493 ms |   2.941 ms |       400 B |
| XenoContext             | 100000      |   3.145 ms | 0.1191 ms | 0.3219 ms |   3.183 ms |    983624 B |
| ArchContext             | 100000      |   3.428 ms | 0.0474 ms | 0.0396 ms |   3.413 ms |      1072 B |
| MorpehContext           | 100000      |   4.277 ms | 0.0563 ms | 0.0499 ms |   4.287 ms |       400 B |
| DefaultECSContext       | 100000      |   5.667 ms | 0.0389 ms | 0.0325 ms |   5.667 ms |       400 B |
| FrifloContext           | 100000      |   6.202 ms | 0.0764 ms | 0.0750 ms |   6.180 ms |       400 B |
| LeoEcsLiteContext       | 100000      |   6.365 ms | 0.0386 ms | 0.0342 ms |   6.369 ms |       400 B |
| LeoEcsContext           | 100000      |   8.328 ms | 0.0467 ms | 0.0754 ms |   8.312 ms |       400 B |
| TinyEcsContext          | 100000      |  31.877 ms | 2.4838 ms | 7.2059 ms |  27.851 ms |  63432560 B |
| FennecsContext          | 100000      | 129.669 ms | 2.5857 ms | 4.5961 ms | 128.397 ms | 184800400 B |
| FlecsNETContext         | 100000      | 238.407 ms | 1.4350 ms | 1.1983 ms | 238.742 ms |       400 B |

# CreateEntityWith3RandomComponents

| Context                 | EntityCount | ChunkSize |       Mean |     Error |    StdDev |     Median |   Allocated |
|-------------------------|-------------|-----------|-----------:|----------:|----------:|-----------:|------------:|
| 📍 **StaticEcsContext** | 100000      | 1         |   4.571 ms | 0.2162 ms | 0.6063 ms |   4.519 ms |       400 B |
| XenoContext             | 100000      | 1         |   6.515 ms | 0.1664 ms | 0.4527 ms |   6.346 ms |       400 B |
| MassiveEcsContext       | 100000      | 1         |   6.774 ms | 0.1020 ms | 0.1965 ms |   6.774 ms |   1089712 B |
| DragonECSContext        | 100000      | 1         |   7.129 ms | 0.1219 ms | 0.2598 ms |   7.074 ms |       400 B |
| ArchContext             | 100000      | 1         |   9.097 ms | 0.1812 ms | 0.5141 ms |   9.112 ms |  20043408 B |
| MorpehContext           | 100000      | 1         |   9.274 ms | 0.2792 ms | 0.7828 ms |   9.199 ms |       400 B |
| FrifloContext           | 100000      | 1         |   9.732 ms | 0.1897 ms | 0.2721 ms |   9.624 ms | 301990264 B |
| DefaultECSContext       | 100000      | 1         |  10.151 ms | 0.1943 ms | 0.3880 ms |  10.057 ms |   3200400 B |
| LeoEcsLiteContext       | 100000      | 1         |  13.739 ms | 0.2603 ms | 0.3384 ms |  13.718 ms |       400 B |
| LeoEcsContext           | 100000      | 1         |  28.651 ms | 0.5729 ms | 1.5092 ms |  28.469 ms |   8800400 B |
| TinyEcsContext          | 100000      | 1         |  36.434 ms | 2.0898 ms | 6.1618 ms |  33.355 ms |  63498432 B |
| FennecsContext          | 100000      | 1         | 135.064 ms | 2.6782 ms | 5.4101 ms | 134.006 ms | 184800400 B |
| FlecsNETContext         | 100000      | 1         | 312.287 ms | 5.3448 ms | 4.9995 ms | 310.534 ms |       400 B |
|                         |             |           |            |           |           |            |             |
| 📍 **StaticEcsContext** | 100000      | 4         |   2.947 ms | 0.1476 ms | 0.4090 ms |   2.945 ms |        64 B |
| XenoContext             | 100000      | 4         |   4.293 ms | 0.0844 ms | 0.2209 ms |   4.242 ms |       400 B |
| MassiveEcsContext       | 100000      | 4         |   4.832 ms | 0.0959 ms | 0.2085 ms |   4.814 ms |   1371920 B |
| DragonECSContext        | 100000      | 4         |   5.529 ms | 0.1402 ms | 0.3885 ms |   5.505 ms |       400 B |
| MorpehContext           | 100000      | 4         |   6.647 ms | 0.1972 ms | 0.5297 ms |   6.575 ms |       400 B |
| ArchContext             | 100000      | 4         |   7.774 ms | 0.2188 ms | 0.6278 ms |   7.757 ms |  20400736 B |
| FrifloContext           | 100000      | 4         |   8.162 ms | 0.1617 ms | 0.1862 ms |   8.141 ms |       400 B |
| DefaultECSContext       | 100000      | 4         |   8.558 ms | 0.1641 ms | 0.3533 ms |   8.481 ms |   3200400 B |
| LeoEcsLiteContext       | 100000      | 4         |  12.245 ms | 0.2446 ms | 0.4153 ms |  12.095 ms |       400 B |
| LeoEcsContext           | 100000      | 4         |  25.372 ms | 0.4982 ms | 0.9833 ms |  25.145 ms |   8800400 B |
| TinyEcsContext          | 100000      | 4         |  33.695 ms | 2.3476 ms | 6.9219 ms |  29.459 ms |  63498432 B |
| FennecsContext          | 100000      | 4         | 131.051 ms | 2.5377 ms | 5.2407 ms | 129.989 ms | 184800400 B |
| FlecsNETContext         | 100000      | 4         | 274.014 ms | 5.2392 ms | 4.3750 ms | 271.908 ms |       400 B |
|                         |             |           |            |           |           |            |             |
| 📍 **StaticEcsContext** | 100000      | 32        |   2.569 ms | 0.1506 ms | 0.4122 ms |   2.543 ms |        64 B |
| XenoContext             | 100000      | 32        |   3.823 ms | 0.0855 ms | 0.2311 ms |   3.778 ms |       400 B |
| MassiveEcsContext       | 100000      | 32        |   4.262 ms | 0.0902 ms | 0.2345 ms |   4.233 ms |   1391720 B |
| DragonECSContext        | 100000      | 32        |   4.947 ms | 0.1185 ms | 0.3224 ms |   4.867 ms |       400 B |
| MorpehContext           | 100000      | 32        |   5.724 ms | 0.1081 ms | 0.2771 ms |   5.702 ms |        64 B |
| FrifloContext           | 100000      | 32        |   7.049 ms | 0.2067 ms | 0.5694 ms |   6.877 ms |       400 B |
| ArchContext             | 100000      | 32        |   7.487 ms | 0.2016 ms | 0.5519 ms |   7.447 ms |  20453488 B |
| DefaultECSContext       | 100000      | 32        |   8.082 ms | 0.1388 ms | 0.3326 ms |   8.011 ms |   3200400 B |
| LeoEcsLiteContext       | 100000      | 32        |  11.347 ms | 0.2036 ms | 0.4994 ms |  11.139 ms |       400 B |
| LeoEcsContext           | 100000      | 32        |  21.864 ms | 0.5689 ms | 1.5184 ms |  21.392 ms |   8800400 B |
| TinyEcsContext          | 100000      | 32        |  31.655 ms | 2.2767 ms | 6.7130 ms |  28.097 ms |  63498144 B |
| FennecsContext          | 100000      | 32        | 131.383 ms | 2.5549 ms | 3.2312 ms | 130.213 ms | 184800400 B |
| FlecsNETContext         | 100000      | 32        | 263.063 ms | 4.5175 ms | 4.0046 ms | 262.505 ms |       400 B |

# CreateEntityWith4Components

| Context                 | EntityCount |       Mean |     Error |    StdDev |     Median |   Allocated |
|-------------------------|-------------|-----------:|----------:|----------:|-----------:|------------:|
| 📍 **StaticEcsContext** | 100000      |   1.797 ms | 0.0971 ms | 0.2708 ms |   1.742 ms |       400 B |
| MassiveEcsContext       | 100000      |   3.070 ms | 0.0101 ms | 0.0084 ms |   3.069 ms |       400 B |
| DragonECSContext        | 100000      |   3.640 ms | 0.0691 ms | 0.0577 ms |   3.645 ms |       400 B |
| ArchContext             | 100000      |   3.669 ms | 0.0685 ms | 0.0535 ms |   3.653 ms |      1072 B |
| XenoContext             | 100000      |   4.427 ms | 0.2882 ms | 0.8362 ms |   4.099 ms |    983336 B |
| MorpehContext           | 100000      |   5.364 ms | 0.0531 ms | 0.0471 ms |   5.372 ms |       400 B |
| DefaultECSContext       | 100000      |   7.052 ms | 0.0377 ms | 0.0315 ms |   7.058 ms |       400 B |
| FrifloContext           | 100000      |   8.796 ms | 0.1698 ms | 0.2086 ms |   8.760 ms |       400 B |
| LeoEcsLiteContext       | 100000      |   9.488 ms | 0.1886 ms | 0.3496 ms |   9.397 ms |       400 B |
| LeoEcsContext           | 100000      |  10.997 ms | 0.1862 ms | 0.1555 ms |  11.043 ms |        64 B |
| TinyEcsContext          | 100000      |  36.615 ms | 2.5843 ms | 7.5792 ms |  32.345 ms |  15746800 B |
| FennecsContext          | 100000      | 182.442 ms | 1.8639 ms | 1.4552 ms | 182.417 ms | 268800400 B |
| FlecsNETContext         | 100000      | 306.803 ms | 4.2538 ms | 3.7709 ms | 305.643 ms |       400 B |

# DeleteEntity

| Context                 | EntityCount |        Mean |     Error |    StdDev |     Median | Allocated |
|-------------------------|-------------|------------:|----------:|----------:|-----------:|----------:|
| FlecsNETContext         | 100000      |    82.72 ns |  14.44 ns |  38.05 ns |   100.0 ns |      64 B |
| 📍 **StaticEcsContext** | 100000      |   111.70 ns |  30.35 ns |  86.58 ns |   100.0 ns |     112 B |
| MassiveEcsContext       | 100000      |   131.18 ns |  23.81 ns |  67.53 ns |   100.0 ns |     400 B |
| LeoEcsLiteContext       | 100000      |   200.00 ns |  26.34 ns |  73.44 ns |   200.0 ns |     400 B |
| DragonECSContext        | 100000      |   204.44 ns |  52.39 ns | 146.04 ns |   200.0 ns |     400 B |
| ArchContext             | 100000      |   424.00 ns | 115.40 ns | 340.27 ns |   300.0 ns |    1072 B |
| FennecsContext          | 100000      |   474.74 ns |  82.66 ns | 239.81 ns |   450.0 ns |     112 B |
| XenoContext             | 100000      |   580.90 ns |  96.39 ns | 267.09 ns |   500.0 ns |     136 B |
| LeoEcsContext           | 100000      |   839.36 ns | 136.69 ns | 389.99 ns |   800.0 ns |     400 B |
| DefaultECSContext       | 100000      | 1,287.37 ns | 152.11 ns | 436.44 ns | 1,200.0 ns |     400 B |
| MorpehContext           | 100000      | 1,496.67 ns | 142.43 ns | 397.03 ns | 1,400.0 ns |     400 B |
| FrifloContext           | 100000      | 1,592.55 ns | 136.01 ns | 388.06 ns | 1,500.0 ns |      88 B |
| TinyEcsContext          | 100000      | 2,209.34 ns | 245.41 ns | 688.15 ns | 2,050.0 ns |     136 B |

# FourRemoveOneComponent

| Context                 | EntityCount |        Mean |       Error |      StdDev |      Median |  Allocated |
|-------------------------|-------------|------------:|------------:|------------:|------------:|-----------:|
| 📍 **StaticEcsContext** | 100000      |    537.8 μs |     5.88 μs |     4.91 μs |    539.0 μs |      400 B |
| MassiveEcsContext       | 100000      |    666.9 μs |     5.41 μs |     4.51 μs |    666.7 μs |      400 B |
| TinyEcsContext          | 100000      |    803.2 μs |    22.91 μs |    63.88 μs |    773.9 μs |      400 B |
| DragonECSContext        | 100000      |  1,054.1 μs |    39.53 μs |   108.21 μs |  1,022.0 μs |      112 B |
| DefaultECSContext       | 100000      |  1,111.4 μs |    16.67 μs |    13.92 μs |  1,112.8 μs |      400 B |
| FrifloContext           | 100000      |  2,794.5 μs |    38.13 μs |    31.84 μs |  2,788.4 μs |      400 B |
| MorpehContext           | 100000      |  2,958.1 μs |    58.36 μs |   142.04 μs |  2,932.6 μs |      400 B |
| XenoContext             | 100000      |  3,116.7 μs |    48.40 μs |    69.41 μs |  3,091.3 μs |   983696 B |
| LeoEcsLiteContext       | 100000      |  3,352.9 μs |    45.67 μs |    35.66 μs |  3,349.9 μs |      400 B |
| LeoEcsContext           | 100000      |  6,744.5 μs |   101.62 μs |    90.08 μs |  6,733.9 μs |      400 B |
| ArchContext             | 100000      | 40,207.3 μs |   517.43 μs |   484.01 μs | 40,015.7 μs | 25603864 B |
| FennecsContext          | 100000      | 56,941.4 μs | 1,078.00 μs | 1,401.71 μs | 56,559.5 μs | 65600064 B |
| FlecsNETContext         | 100000      | 70,826.9 μs | 1,041.69 μs |   923.43 μs | 71,080.1 μs |      400 B |

# FourRemoveThreeComponents

| Context                 | EntityCount |       Mean |     Error |    StdDev |     Median |   Allocated |
|-------------------------|-------------|-----------:|----------:|----------:|-----------:|------------:|
| 📍 **StaticEcsContext** | 100000      |   1.777 ms | 0.0121 ms | 0.0094 ms |   1.781 ms |       400 B |
| MassiveEcsContext       | 100000      |   1.976 ms | 0.0376 ms | 0.0333 ms |   1.966 ms |       400 B |
| DragonECSContext        | 100000      |   2.517 ms | 0.0496 ms | 0.1139 ms |   2.479 ms |       400 B |
| MorpehContext           | 100000      |   2.815 ms | 0.0472 ms | 0.0369 ms |   2.823 ms |       400 B |
| DefaultECSContext       | 100000      |   3.202 ms | 0.0491 ms | 0.0410 ms |   3.197 ms |        64 B |
| XenoContext             | 100000      |   5.023 ms | 0.0914 ms | 0.0714 ms |   5.018 ms |    983688 B |
| FrifloContext           | 100000      |   6.078 ms | 0.0872 ms | 0.0728 ms |   6.053 ms |        64 B |
| TinyEcsContext          | 100000      |   7.535 ms | 2.2204 ms | 6.5121 ms |   4.167 ms |        64 B |
| LeoEcsLiteContext       | 100000      |   8.422 ms | 0.1672 ms | 0.1717 ms |   8.383 ms |       400 B |
| LeoEcsContext           | 100000      |  15.123 ms | 0.2107 ms | 0.1645 ms |  15.091 ms |       400 B |
| ArchContext             | 100000      |  43.966 ms | 0.8360 ms | 0.8585 ms |  43.823 ms |  25609048 B |
| FennecsContext          | 100000      | 136.602 ms | 1.1793 ms | 0.9848 ms | 136.556 ms | 162400064 B |
| FlecsNETContext         | 100000      | 157.208 ms | 1.3868 ms | 1.1581 ms | 157.370 ms |       400 B |

# FourRemoveTwoComponents

| Context                 | EntityCount |       Mean |     Error |    StdDev |     Median |   Allocated |
|-------------------------|-------------|-----------:|----------:|----------:|-----------:|------------:|
| 📍 **StaticEcsContext** | 100000      |   1.133 ms | 0.0093 ms | 0.0078 ms |   1.131 ms |       400 B |
| MassiveEcsContext       | 100000      |   1.290 ms | 0.0088 ms | 0.0068 ms |   1.290 ms |       400 B |
| DragonECSContext        | 100000      |   1.733 ms | 0.0316 ms | 0.0681 ms |   1.709 ms |       400 B |
| DefaultECSContext       | 100000      |   2.134 ms | 0.0114 ms | 0.0101 ms |   2.133 ms |       400 B |
| TinyEcsContext          | 100000      |   2.581 ms | 0.0444 ms | 0.0371 ms |   2.577 ms |       112 B |
| MorpehContext           | 100000      |   3.350 ms | 0.0670 ms | 0.1877 ms |   3.291 ms |       400 B |
| XenoContext             | 100000      |   3.580 ms | 0.0582 ms | 0.1240 ms |   3.541 ms |    983688 B |
| FrifloContext           | 100000      |   4.436 ms | 0.0353 ms | 0.0295 ms |   4.433 ms |       400 B |
| LeoEcsLiteContext       | 100000      |   6.464 ms | 0.0632 ms | 0.0560 ms |   6.462 ms |       400 B |
| LeoEcsContext           | 100000      |  10.732 ms | 0.2106 ms | 0.1969 ms |  10.711 ms |       112 B |
| ArchContext             | 100000      |  41.328 ms | 0.6571 ms | 0.6146 ms |  41.132 ms |  25609336 B |
| FennecsContext          | 100000      | 104.582 ms | 1.6727 ms | 1.5647 ms | 103.810 ms | 118400400 B |
| FlecsNETContext         | 100000      | 122.086 ms | 1.0503 ms | 0.9311 ms | 122.197 ms |       400 B |

# OneAddOneComponent

| Context                 | EntityCount |        Mean |     Error |      StdDev |  Allocated |
|-------------------------|-------------|------------:|----------:|------------:|-----------:|
| 📍 **StaticEcsContext** | 100000      |    400.0 μs |   4.71 μs |     3.93 μs |      400 B |
| MassiveEcsContext       | 100000      |    725.4 μs |   3.41 μs |     5.20 μs |      112 B |
| DragonECSContext        | 100000      |  1,046.3 μs |  29.06 μs |    83.37 μs |      400 B |
| DefaultECSContext       | 100000      |  1,400.0 μs |  22.78 μs |    19.02 μs |      112 B |
| LeoEcsLiteContext       | 100000      |  1,650.5 μs |  29.48 μs |    70.07 μs |      112 B |
| FrifloContext           | 100000      |  2,075.9 μs |  40.84 μs |    54.52 μs |      400 B |
| MorpehContext           | 100000      |  2,956.4 μs |  59.13 μs |   137.04 μs |      400 B |
| XenoContext             | 100000      |  3,193.4 μs |  57.20 μs |   106.02 μs |   983688 B |
| LeoEcsContext           | 100000      |  3,815.7 μs |  58.69 μs |    57.64 μs |      112 B |
| TinyEcsContext          | 100000      |  3,960.1 μs |  79.18 μs |   175.45 μs |  2400400 B |
| ArchContext             | 100000      |  7,595.3 μs | 151.62 μs |   292.12 μs |     1408 B |
| FennecsContext          | 100000      | 44,202.2 μs | 872.84 μs | 1,551.47 μs | 64000112 B |
| FlecsNETContext         | 100000      | 87,521.5 μs | 845.26 μs |   749.31 μs |      400 B |

# OneAddThreeComponents

| Context                 | EntityCount |       Mean |      Error |     StdDev |     Median |   Allocated |
|-------------------------|-------------|-----------:|-----------:|-----------:|-----------:|------------:|
| 📍 **StaticEcsContext** | 100000      |   1.094 ms |  0.0131 ms |  0.0116 ms |   1.091 ms |       400 B |
| DragonECSContext        | 100000      |   1.961 ms |  0.0383 ms |  0.0470 ms |   1.957 ms |       400 B |
| MassiveEcsContext       | 100000      |   2.051 ms |  0.0285 ms |  0.0253 ms |   2.049 ms |       112 B |
| DefaultECSContext       | 100000      |   4.077 ms |  0.0391 ms |  0.0326 ms |   4.080 ms |       400 B |
| XenoContext             | 100000      |   4.332 ms |  0.0735 ms |  0.1417 ms |   4.300 ms |    983360 B |
| MorpehContext           | 100000      |   4.812 ms |  0.2834 ms |  0.7995 ms |   4.528 ms |       400 B |
| LeoEcsLiteContext       | 100000      |   6.021 ms |  0.0718 ms |  0.0600 ms |   6.005 ms |       400 B |
| FrifloContext           | 100000      |   6.615 ms |  0.0561 ms |  0.0468 ms |   6.601 ms |       400 B |
| LeoEcsContext           | 100000      |   8.808 ms |  0.1722 ms |  0.1526 ms |   8.771 ms |       400 B |
| ArchContext             | 100000      |   9.833 ms |  0.1953 ms |  0.5077 ms |   9.619 ms |      1744 B |
| TinyEcsContext          | 100000      |  28.543 ms |  2.6022 ms |  7.5078 ms |  24.085 ms |   7200400 B |
| FennecsContext          | 100000      | 211.036 ms | 12.9668 ms | 38.2328 ms | 219.107 ms | 219200400 B |
| FlecsNETContext         | 100000      | 270.997 ms |  3.8597 ms |  3.0134 ms | 270.662 ms |       400 B |

# OneAddTwoComponents

| Context                 | EntityCount |         Mean |       Error |      StdDev |   Allocated |
|-------------------------|-------------|-------------:|------------:|------------:|------------:|
| 📍 **StaticEcsContext** | 100000      |     726.1 μs |    11.88 μs |    10.53 μs |       400 B |
| DragonECSContext        | 100000      |   1,370.5 μs |    30.69 μs |    85.54 μs |       400 B |
| MassiveEcsContext       | 100000      |   1,449.7 μs |    13.49 μs |    11.96 μs |       400 B |
| DefaultECSContext       | 100000      |   2,776.1 μs |    49.02 μs |    52.45 μs |       112 B |
| MorpehContext           | 100000      |   3,406.8 μs |    67.71 μs |    83.15 μs |       112 B |
| LeoEcsLiteContext       | 100000      |   3,599.0 μs |    26.17 μs |    20.43 μs |       400 B |
| XenoContext             | 100000      |   3,693.2 μs |    73.50 μs |   150.13 μs |    983408 B |
| FrifloContext           | 100000      |   3,992.5 μs |    60.80 μs |   109.63 μs |       400 B |
| LeoEcsContext           | 100000      |   6,473.9 μs |   124.96 μs |   128.33 μs |       400 B |
| TinyEcsContext          | 100000      |   7,535.4 μs |   150.29 μs |   154.34 μs |   4800400 B |
| ArchContext             | 100000      |   9,098.8 μs |   126.25 μs |   105.43 μs |      1744 B |
| FennecsContext          | 100000      |  98,205.4 μs | 1,942.82 μs | 3,024.74 μs | 135200112 B |
| FlecsNETContext         | 100000      | 192,712.4 μs | 1,920.44 μs | 1,796.38 μs |       400 B |

# Remove1ComponentRandomOrder

| Context                 | EntityCount |      Mean |     Error |    StdDev |    Median |  Allocated |
|-------------------------|-------------|----------:|----------:|----------:|----------:|-----------:|
| MassiveEcsContext       | 100000      |  1.246 ms | 0.0389 ms | 0.1038 ms |  1.232 ms |      400 B |
| 📍 **StaticEcsContext** | 100000      |  1.532 ms | 0.0583 ms | 0.1691 ms |  1.543 ms |      400 B |
| FrifloContext           | 100000      |  4.344 ms | 0.2894 ms | 0.8020 ms |  4.068 ms |      400 B |
| DragonECSContext        | 100000      |  5.500 ms | 0.1087 ms | 0.2767 ms |  5.411 ms |      400 B |
| LeoEcsLiteContext       | 100000      |  5.789 ms | 0.2558 ms | 0.7174 ms |  5.794 ms |      400 B |
| XenoContext             | 100000      |  7.088 ms | 0.3628 ms | 1.0115 ms |  6.933 ms |      400 B |
| DefaultECSContext       | 100000      |  8.670 ms | 0.1727 ms | 0.4172 ms |  8.582 ms |      400 B |
| TinyEcsContext          | 100000      | 17.602 ms | 0.3476 ms | 0.6268 ms | 17.599 ms |  1575280 B |
| MorpehContext           | 100000      | 18.659 ms | 0.3109 ms | 0.2756 ms | 18.735 ms |      400 B |
| LeoEcsContext           | 100000      | 30.748 ms | 0.4731 ms | 0.4858 ms | 30.824 ms |      400 B |
| FennecsContext          | 100000      | 36.537 ms | 0.5424 ms | 0.4529 ms | 36.559 ms | 36800400 B |
| ArchContext             | 100000      | 37.293 ms | 0.5963 ms | 0.5578 ms | 37.247 ms | 27291216 B |
| FlecsNETContext         | 100000      | 77.743 ms | 1.3897 ms | 1.2999 ms | 77.646 ms |      400 B |

# Remove1Component

| Context                 | EntityCount |        Mean |       Error |      StdDev |  Allocated |
|-------------------------|-------------|------------:|------------:|------------:|-----------:|
| 📍 **StaticEcsContext** | 100000      |    538.7 μs |     1.65 μs |     1.38 μs |      400 B |
| MassiveEcsContext       | 100000      |    661.3 μs |     3.75 μs |     7.48 μs |      400 B |
| DefaultECSContext       | 100000      |  1,140.8 μs |    21.15 μs |    20.77 μs |      400 B |
| FrifloContext           | 100000      |  1,511.1 μs |    15.96 μs |    13.33 μs |      400 B |
| LeoEcsLiteContext       | 100000      |  1,860.4 μs |    28.91 μs |    58.41 μs |      400 B |
| MorpehContext           | 100000      |  2,641.9 μs |    49.94 μs |   118.68 μs |      400 B |
| XenoContext             | 100000      |  2,824.1 μs |    48.19 μs |    64.33 μs | 33554520 B |
| LeoEcsContext           | 100000      |  3,929.0 μs |    64.11 μs |   136.62 μs |      400 B |
| DragonECSContext        | 100000      |  3,994.0 μs |    42.07 μs |    35.13 μs |      400 B |
| TinyEcsContext          | 100000      |  5,424.6 μs |    99.24 μs |   181.47 μs |  1640864 B |
| FennecsContext          | 100000      | 27,710.2 μs |   551.61 μs | 1,101.62 μs | 36800400 B |
| ArchContext             | 100000      | 30,906.9 μs |   448.64 μs |   374.63 μs | 26622024 B |
| FlecsNETContext         | 100000      | 53,167.7 μs | 1,056.50 μs | 1,548.61 μs |      400 B |

# Remove2ComponentsRandomOrder

| Context                 | EntityCount |       Mean |     Error |    StdDev |     Median |  Allocated |
|-------------------------|-------------|-----------:|----------:|----------:|-----------:|-----------:|
| 📍 **StaticEcsContext** | 100000      |   3.606 ms | 0.1697 ms | 0.4730 ms |   3.453 ms |      400 B |
| MassiveEcsContext       | 100000      |   3.658 ms | 0.1728 ms | 0.4931 ms |   3.575 ms |      400 B |
| FrifloContext           | 100000      |   7.397 ms | 0.3666 ms | 1.0578 ms |   7.236 ms |      400 B |
| DragonECSContext        | 100000      |  10.689 ms | 0.2126 ms | 0.5820 ms |  10.517 ms |      400 B |
| LeoEcsLiteContext       | 100000      |  11.023 ms | 0.5708 ms | 1.6741 ms |  10.850 ms |      400 B |
| XenoContext             | 100000      |  11.569 ms | 0.2480 ms | 0.6789 ms |  11.497 ms |      400 B |
| DefaultECSContext       | 100000      |  16.950 ms | 0.3319 ms | 0.9085 ms |  16.649 ms |      400 B |
| TinyEcsContext          | 100000      |  25.209 ms | 0.4524 ms | 0.4010 ms |  25.197 ms |  1575280 B |
| MorpehContext           | 100000      |  28.644 ms | 0.6975 ms | 1.9901 ms |  28.178 ms |      400 B |
| LeoEcsContext           | 100000      |  38.048 ms | 0.7480 ms | 1.2702 ms |  38.374 ms |      400 B |
| ArchContext             | 100000      |  42.877 ms | 0.6798 ms | 0.6026 ms |  43.084 ms | 27308432 B |
| FennecsContext          | 100000      |  76.045 ms | 1.1076 ms | 0.9818 ms |  75.699 ms | 75200400 B |
| FlecsNETContext         | 100000      | 132.452 ms | 0.6242 ms | 0.4873 ms | 132.457 ms |      400 B |

# Remove2Components

| Context                 | EntityCount |      Mean |     Error |    StdDev |    Median |  Allocated |
|-------------------------|-------------|----------:|----------:|----------:|----------:|-----------:|
| 📍 **StaticEcsContext** | 100000      |  1.043 ms | 0.0049 ms | 0.0038 ms |  1.042 ms |      400 B |
| MassiveEcsContext       | 100000      |  1.253 ms | 0.0233 ms | 0.0525 ms |  1.273 ms |      400 B |
| DefaultECSContext       | 100000      |  2.119 ms | 0.0120 ms | 0.0100 ms |  2.116 ms |      400 B |
| FrifloContext           | 100000      |  2.958 ms | 0.0538 ms | 0.0449 ms |  2.943 ms |      400 B |
| MorpehContext           | 100000      |  3.141 ms | 0.0627 ms | 0.1619 ms |  3.113 ms |      400 B |
| XenoContext             | 100000      |  3.312 ms | 0.0551 ms | 0.0755 ms |  3.294 ms |      400 B |
| LeoEcsLiteContext       | 100000      |  3.354 ms | 0.0562 ms | 0.0498 ms |  3.341 ms |      400 B |
| LeoEcsContext           | 100000      |  6.502 ms | 0.1193 ms | 0.2327 ms |  6.435 ms |      400 B |
| DragonECSContext        | 100000      |  6.503 ms | 0.0375 ms | 0.0313 ms |  6.497 ms |      400 B |
| TinyEcsContext          | 100000      | 10.571 ms | 0.1931 ms | 0.1806 ms | 10.569 ms |  1575280 B |
| ArchContext             | 100000      | 34.924 ms | 0.2919 ms | 0.2587 ms | 34.907 ms | 27780560 B |
| FennecsContext          | 100000      | 61.137 ms | 1.1247 ms | 0.9392 ms | 60.983 ms | 75200400 B |
| FlecsNETContext         | 100000      | 98.938 ms | 1.0412 ms | 0.9230 ms | 98.941 ms |      400 B |

# Remove3ComponentsRandomOrder

| Context                 | EntityCount |       Mean |     Error |    StdDev |     Median |   Allocated |
|-------------------------|-------------|-----------:|----------:|----------:|-----------:|------------:|
| MassiveEcsContext       | 100000      |   7.228 ms | 0.5104 ms | 1.4807 ms |   7.229 ms |       400 B |
| 📍 **StaticEcsContext** | 100000      |   8.317 ms | 0.6452 ms | 1.8921 ms |   8.546 ms |       400 B |
| FrifloContext           | 100000      |  12.576 ms | 1.3173 ms | 3.8217 ms |  10.874 ms |       400 B |
| DragonECSContext        | 100000      |  13.959 ms | 0.2759 ms | 0.5759 ms |  13.970 ms |       400 B |
| LeoEcsLiteContext       | 100000      |  16.151 ms | 0.5088 ms | 1.4843 ms |  15.883 ms |       400 B |
| XenoContext             | 100000      |  20.472 ms | 1.0471 ms | 3.0378 ms |  19.344 ms |        64 B |
| DefaultECSContext       | 100000      |  24.878 ms | 0.4906 ms | 1.0666 ms |  24.629 ms |       400 B |
| MorpehContext           | 100000      |  30.405 ms | 0.5949 ms | 0.7941 ms |  30.441 ms |       400 B |
| TinyEcsContext          | 100000      |  33.412 ms | 0.9291 ms | 2.6357 ms |  32.192 ms |   1575280 B |
| LeoEcsContext           | 100000      |  49.484 ms | 0.9644 ms | 0.9903 ms |  49.436 ms |       400 B |
| ArchContext             | 100000      |  50.051 ms | 0.9708 ms | 1.1922 ms |  50.360 ms |  29477128 B |
| FennecsContext          | 100000      | 127.649 ms | 0.9635 ms | 0.7523 ms | 127.568 ms | 128000400 B |
| FlecsNETContext         | 100000      | 188.357 ms | 3.7546 ms | 3.8557 ms | 188.120 ms |       400 B |

# Remove3Components

| Context                 | EntityCount |       Mean |     Error |    StdDev |   Allocated |
|-------------------------|-------------|-----------:|----------:|----------:|------------:|
| 📍 **StaticEcsContext** | 100000      |   1.546 ms | 0.0222 ms | 0.0197 ms |       400 B |
| MassiveEcsContext       | 100000      |   1.964 ms | 0.0371 ms | 0.0532 ms |       400 B |
| DragonECSContext        | 100000      |   2.588 ms | 0.0281 ms | 0.0594 ms |       400 B |
| MorpehContext           | 100000      |   2.785 ms | 0.0410 ms | 0.0383 ms |       400 B |
| DefaultECSContext       | 100000      |   3.252 ms | 0.0412 ms | 0.0365 ms |       400 B |
| XenoContext             | 100000      |   3.802 ms | 0.0695 ms | 0.0773 ms |       400 B |
| FrifloContext           | 100000      |   4.757 ms | 0.0636 ms | 0.0531 ms |       400 B |
| LeoEcsLiteContext       | 100000      |   5.516 ms | 0.0438 ms | 0.0366 ms |       400 B |
| LeoEcsContext           | 100000      |   9.783 ms | 0.1271 ms | 0.1061 ms |       400 B |
| TinyEcsContext          | 100000      |  14.728 ms | 0.1655 ms | 0.1548 ms |   1575280 B |
| ArchContext             | 100000      |  41.484 ms | 0.6346 ms | 0.5299 ms |  28563952 B |
| FennecsContext          | 100000      | 100.291 ms | 1.5630 ms | 2.4335 ms | 128000400 B |
| FlecsNETContext         | 100000      | 141.363 ms | 0.8199 ms | 0.7268 ms |       400 B |

# Remove4ComponentsRandomOrder

| Context                 | EntityCount |      Mean |    Error |   StdDev |    Median |   Allocated |
|-------------------------|-------------|----------:|---------:|---------:|----------:|------------:|
| MassiveEcsContext       | 100000      |  11.82 ms | 0.891 ms | 2.600 ms |  11.10 ms |       400 B |
| 📍 **StaticEcsContext** | 100000      |  13.60 ms | 0.700 ms | 2.041 ms |  13.05 ms |       400 B |
| FrifloContext           | 100000      |  16.68 ms | 0.834 ms | 2.459 ms |  16.38 ms |       400 B |
| DragonECSContext        | 100000      |  22.42 ms | 0.752 ms | 2.193 ms |  21.91 ms |       400 B |
| LeoEcsLiteContext       | 100000      |  22.56 ms | 0.600 ms | 1.760 ms |  22.46 ms |       400 B |
| XenoContext             | 100000      |  25.95 ms | 0.588 ms | 1.688 ms |  25.83 ms |       400 B |
| DefaultECSContext       | 100000      |  34.38 ms | 0.887 ms | 2.517 ms |  33.15 ms |       400 B |
| TinyEcsContext          | 100000      |  40.30 ms | 0.676 ms | 0.600 ms |  40.30 ms |   1575280 B |
| MorpehContext           | 100000      |  46.90 ms | 0.863 ms | 1.265 ms |  47.03 ms |       400 B |
| ArchContext             | 100000      |  53.10 ms | 0.728 ms | 0.681 ms |  53.04 ms |  29038336 B |
| LeoEcsContext           | 100000      |  65.09 ms | 1.299 ms | 3.491 ms |  65.55 ms |       400 B |
| FennecsContext          | 100000      | 198.00 ms | 3.923 ms | 5.626 ms | 199.36 ms | 193600400 B |
| FlecsNETContext         | 100000      | 236.43 ms | 4.529 ms | 5.216 ms | 233.97 ms |       400 B |

# Remove4Components

| Context                 | EntityCount |       Mean |     Error |    StdDev |   Allocated |
|-------------------------|-------------|-----------:|----------:|----------:|------------:|
| 📍 **StaticEcsContext** | 100000      |   2.024 ms | 0.0268 ms | 0.0209 ms |       400 B |
| MassiveEcsContext       | 100000      |   2.569 ms | 0.0280 ms | 0.0234 ms |       400 B |
| DragonECSContext        | 100000      |   3.376 ms | 0.0509 ms | 0.0930 ms |       400 B |
| DefaultECSContext       | 100000      |   4.326 ms | 0.0207 ms | 0.0184 ms |       112 B |
| MorpehContext           | 100000      |   4.626 ms | 0.0787 ms | 0.0736 ms |       112 B |
| XenoContext             | 100000      |   4.642 ms | 0.0919 ms | 0.1226 ms |       400 B |
| LeoEcsLiteContext       | 100000      |   6.865 ms | 0.0786 ms | 0.0656 ms |       400 B |
| FrifloContext           | 100000      |   7.414 ms | 0.1205 ms | 0.0941 ms |       400 B |
| LeoEcsContext           | 100000      |  13.718 ms | 0.0946 ms | 0.0885 ms |        64 B |
| TinyEcsContext          | 100000      |  22.700 ms | 0.3614 ms | 0.3380 ms |   1575280 B |
| ArchContext             | 100000      |  43.300 ms | 0.7010 ms | 0.6557 ms |  29211080 B |
| FennecsContext          | 100000      | 158.750 ms | 3.1388 ms | 3.4887 ms | 193600112 B |
| FlecsNETContext         | 100000      | 179.173 ms | 1.0148 ms | 0.9492 ms |       400 B |

| Context                 | EntityCount |       Mean |     Error |    StdDev |   Allocated |
|-------------------------|-------------|-----------:|----------:|----------:|------------:|
| 📍 **StaticEcsContext** | 100000      |   2.041 ms | 0.0111 ms | 0.0087 ms |       400 B |
| MassiveEcsContext       | 100000      |   2.572 ms | 0.0176 ms | 0.0147 ms |       400 B |
| DragonECSContext        | 100000      |   3.366 ms | 0.0344 ms | 0.0664 ms |       400 B |
| DefaultECSContext       | 100000      |   4.354 ms | 0.0383 ms | 0.0339 ms |       400 B |
| MorpehContext           | 100000      |   4.733 ms | 0.0942 ms | 0.1675 ms |       400 B |
| XenoContext             | 100000      |   4.763 ms | 0.0949 ms | 0.1982 ms |       400 B |
| FrifloContext           | 100000      |   7.314 ms | 0.0708 ms | 0.0591 ms |       400 B |
| LeoEcsLiteContext       | 100000      |   7.528 ms | 0.1288 ms | 0.1076 ms |       112 B |
| LeoEcsContext           | 100000      |  13.778 ms | 0.0841 ms | 0.0746 ms |       400 B |
| TinyEcsContext          | 100000      |  22.536 ms | 0.3327 ms | 0.2949 ms |   1575280 B |
| ArchContext             | 100000      |  44.850 ms | 0.8836 ms | 1.4762 ms |  31532144 B |
| FennecsContext          | 100000      | 159.326 ms | 2.7385 ms | 3.0438 ms | 193600400 B |
| FlecsNETContext         | 100000      | 179.166 ms | 0.9521 ms | 0.8906 ms |       400 B |

# SystemWith1ComponentMultipleComposition

| Context                 | EntityCount | Padding |        Mean |     Error |     StdDev |      Median | Allocated |
|-------------------------|-------------|---------|------------:|----------:|-----------:|------------:|----------:|
| TinyEcsContext          | 100000      | 0       |    35.54 μs |  6.943 μs |  20.364 μs |    31.60 μs |    2256 B |
| FrifloContext           | 100000      | 0       |   207.49 μs |  1.365 μs |   1.140 μs |   207.20 μs |     232 B |
| LeoEcsContext           | 100000      | 0       |   212.45 μs |  2.675 μs |   5.089 μs |   211.70 μs |     400 B |
| ArchContext             | 100000      | 0       |   216.17 μs |  2.446 μs |   2.288 μs |   216.60 μs |      64 B |
| 📍 **StaticEcsContext** | 100000      | 0       |   224.82 μs |  2.129 μs |   1.778 μs |   224.20 μs |     112 B |
| XenoContext             | 100000      | 0       |   225.56 μs |  2.891 μs |   2.257 μs |   225.70 μs |     112 B |
| MassiveEcsContext       | 100000      | 0       |   234.03 μs |  2.683 μs |   2.240 μs |   234.70 μs |     400 B |
| LeoEcsLiteContext       | 100000      | 0       |   284.25 μs |  2.111 μs |   1.762 μs |   283.90 μs |     400 B |
| FennecsContext          | 100000      | 0       |   335.39 μs | 13.785 μs |  39.992 μs |   331.00 μs |     592 B |
| MorpehContext           | 100000      | 0       |   556.62 μs |  6.412 μs |   5.006 μs |   557.35 μs |     112 B |
| DragonECSContext        | 100000      | 0       |   752.91 μs |  7.184 μs |   5.999 μs |   753.20 μs |     112 B |
| FlecsNETContext         | 100000      | 0       |   836.46 μs | 10.185 μs |   9.029 μs |   834.60 μs |     400 B |
| DefaultECSContext       | 100000      | 0       | 1,834.85 μs | 18.470 μs |  29.825 μs | 1,829.70 μs |     448 B |
|                         |             |         |             |           |            |             |           |
| TinyEcsContext          | 100000      | 10      |    21.72 μs |  4.572 μs |  13.045 μs |    17.90 μs |    2096 B |
| FrifloContext           | 100000      | 10      |   211.78 μs |  2.167 μs |   1.809 μs |   211.00 μs |     520 B |
| XenoContext             | 100000      | 10      |   219.67 μs |  4.399 μs |   7.469 μs |   215.70 μs |     400 B |
| 📍 **StaticEcsContext** | 100000      | 10      |   245.96 μs |  2.306 μs |   2.044 μs |   245.35 μs |     112 B |
| ArchContext             | 100000      | 10      |   254.99 μs |  3.988 μs |   8.056 μs |   253.30 μs |     736 B |
| LeoEcsContext           | 100000      | 10      |   270.55 μs |  5.362 μs |   5.016 μs |   271.40 μs |     400 B |
| LeoEcsLiteContext       | 100000      | 10      |   329.54 μs |  5.835 μs |  11.518 μs |   325.15 μs |     400 B |
| FennecsContext          | 100000      | 10      |   390.59 μs | 40.329 μs | 113.087 μs |   364.30 μs |     592 B |
| MassiveEcsContext       | 100000      | 10      |   400.27 μs | 34.635 μs | 100.481 μs |   458.70 μs |     400 B |
| MorpehContext           | 100000      | 10      |   590.98 μs |  8.119 μs |   7.595 μs |   591.45 μs |     400 B |
| DragonECSContext        | 100000      | 10      |   806.84 μs | 12.604 μs |  10.525 μs |   808.30 μs |     400 B |
| FlecsNETContext         | 100000      | 10      |   851.76 μs | 15.152 μs |  19.162 μs |   851.20 μs |     400 B |
| DefaultECSContext       | 100000      | 10      | 4,044.93 μs | 79.241 μs | 136.686 μs | 3,987.15 μs |     736 B |

# SystemWith1Component

| Context                 | EntityCount | Padding |        Mean |     Error |     StdDev |      Median | Allocated |
|-------------------------|-------------|---------|------------:|----------:|-----------:|------------:|----------:|
| TinyEcsContext          | 100000      | 0       |    29.50 μs |  2.629 μs |   7.628 μs |    29.20 μs |    2480 B |
| FrifloContext           | 100000      | 0       |   207.63 μs |  2.721 μs |   2.412 μs |   207.30 μs |     232 B |
| MassiveEcsContext       | 100000      | 0       |   230.89 μs |  2.207 μs |   1.957 μs |   230.50 μs |     400 B |
| XenoContext             | 100000      | 0       |   236.78 μs |  3.326 μs |   3.111 μs |   236.30 μs |     112 B |
| 📍 **StaticEcsContext** | 100000      | 0       |   246.73 μs |  1.295 μs |   1.212 μs |   246.80 μs |      64 B |
| ArchContext             | 100000      | 0       |   250.88 μs |  4.987 μs |   9.367 μs |   249.10 μs |      64 B |
| LeoEcsContext           | 100000      | 0       |   268.92 μs |  5.012 μs |   4.186 μs |   268.20 μs |     400 B |
| LeoEcsLiteContext       | 100000      | 0       |   288.33 μs |  3.275 μs |   2.734 μs |   287.90 μs |     112 B |
| FennecsContext          | 100000      | 0       |   311.18 μs | 15.806 μs |  45.351 μs |   306.00 μs |     880 B |
| MorpehContext           | 100000      | 0       |   577.17 μs | 11.383 μs |  20.815 μs |   569.25 μs |     400 B |
| DragonECSContext        | 100000      | 0       |   748.18 μs |  6.889 μs |   5.753 μs |   747.30 μs |     400 B |
| FlecsNETContext         | 100000      | 0       |   872.12 μs | 17.341 μs |  31.270 μs |   865.20 μs |     400 B |
| DefaultECSContext       | 100000      | 0       | 1,828.05 μs | 19.467 μs |  31.435 μs | 1,820.40 μs |     736 B |
|                         |             |         |             |           |            |             |           |
| TinyEcsContext          | 100000      | 10      |    17.87 μs |  3.070 μs |   8.809 μs |    13.00 μs |    1984 B |
| ArchContext             | 100000      | 10      |   207.32 μs |  4.140 μs |   9.677 μs |   204.00 μs |     736 B |
| 📍 **StaticEcsContext** | 100000      | 10      |   224.07 μs |  1.480 μs |   1.312 μs |   223.75 μs |     400 B |
| MassiveEcsContext       | 100000      | 10      |   227.58 μs |  1.102 μs |   0.920 μs |   227.70 μs |      64 B |
| FrifloContext           | 100000      | 10      |   229.21 μs |  4.577 μs |  10.237 μs |   230.85 μs |     520 B |
| XenoContext             | 100000      | 10      |   237.30 μs |  4.099 μs |   7.179 μs |   238.90 μs |     400 B |
| LeoEcsContext           | 100000      | 10      |   269.36 μs |  4.333 μs |   3.841 μs |   269.10 μs |     400 B |
| LeoEcsLiteContext       | 100000      | 10      |   398.84 μs | 10.729 μs |  30.085 μs |   392.80 μs |     400 B |
| FennecsContext          | 100000      | 10      |   489.37 μs | 57.539 μs | 167.843 μs |   435.05 μs |     544 B |
| MorpehContext           | 100000      | 10      |   593.46 μs | 10.936 μs |   9.695 μs |   591.30 μs |     112 B |
| DragonECSContext        | 100000      | 10      |   778.82 μs |  7.446 μs |   6.218 μs |   777.70 μs |     400 B |
| FlecsNETContext         | 100000      | 10      |   876.34 μs | 17.266 μs |  38.619 μs |   865.90 μs |     400 B |
| DefaultECSContext       | 100000      | 10      | 4,694.72 μs | 47.047 μs |  39.287 μs | 4,689.55 μs |     736 B |

# SystemWith2ComponentsMultipleComposition

| Context                 | EntityCount | Padding |        Mean |      Error |     StdDev |      Median | Allocated |
|-------------------------|-------------|---------|------------:|-----------:|-----------:|------------:|----------:|
| TinyEcsContext          | 100000      | 0       |    39.49 μs |   5.520 μs |  16.274 μs |    36.75 μs |    2320 B |
| ArchContext             | 100000      | 0       |   260.57 μs |   5.032 μs |   6.717 μs |   259.10 μs |    1120 B |
| FrifloContext           | 100000      | 0       |   310.25 μs |   4.721 μs |  10.753 μs |   308.75 μs |     232 B |
| 📍 **StaticEcsContext** | 100000      | 0       |   357.01 μs |   3.510 μs |   3.112 μs |   356.45 μs |      64 B |
| LeoEcsContext           | 100000      | 0       |   403.33 μs |   7.660 μs |  20.312 μs |   397.35 μs |     400 B |
| LeoEcsLiteContext       | 100000      | 0       |   422.69 μs |   3.736 μs |   3.312 μs |   423.15 μs |     400 B |
| XenoContext             | 100000      | 0       |   439.81 μs |   8.433 μs |   9.712 μs |   442.10 μs |     400 B |
| MassiveEcsContext       | 100000      | 0       |   487.61 μs |   5.970 μs |   9.295 μs |   486.40 μs |     736 B |
| FennecsContext          | 100000      | 0       |   549.44 μs |  26.870 μs |  78.380 μs |   548.35 μs |     912 B |
| MorpehContext           | 100000      | 0       | 1,029.96 μs |  16.249 μs |  19.343 μs | 1,029.70 μs |     112 B |
| DragonECSContext        | 100000      | 0       | 1,354.71 μs |   8.328 μs |   7.790 μs | 1,352.65 μs |     400 B |
| FlecsNETContext         | 100000      | 0       | 1,411.36 μs |  27.158 μs |  29.059 μs | 1,403.75 μs |     400 B |
| DefaultECSContext       | 100000      | 0       | 1,889.04 μs |  37.759 μs |  65.132 μs | 1,874.20 μs |     448 B |
|                         |             |         |             |            |            |             |           |
| TinyEcsContext          | 100000      | 10      |    44.48 μs |   5.459 μs |  16.011 μs |    38.00 μs |    2320 B |
| ArchContext             | 100000      | 10      |   283.42 μs |   7.448 μs |  20.639 μs |   277.20 μs |    1408 B |
| FrifloContext           | 100000      | 10      |   306.41 μs |   6.134 μs |  12.249 μs |   304.60 μs |     520 B |
| LeoEcsContext           | 100000      | 10      |   465.04 μs |   9.035 μs |  14.589 μs |   465.70 μs |     400 B |
| FennecsContext          | 100000      | 10      |   825.20 μs |  80.181 μs | 233.892 μs |   788.80 μs |     912 B |
| 📍 **StaticEcsContext** | 100000      | 10      |   885.56 μs |  17.581 μs |  28.389 μs |   874.20 μs |     400 B |
| MassiveEcsContext       | 100000      | 10      | 1,359.40 μs |  26.709 μs |  46.073 μs | 1,348.35 μs |     400 B |
| FlecsNETContext         | 100000      | 10      | 1,396.11 μs |  25.767 μs |  64.644 μs | 1,380.35 μs |     112 B |
| DragonECSContext        | 100000      | 10      | 1,500.49 μs |  27.237 μs |  22.744 μs | 1,489.10 μs |     400 B |
| LeoEcsLiteContext       | 100000      | 10      | 2,478.61 μs |  16.949 μs |  16.646 μs | 2,478.10 μs |     400 B |
| XenoContext             | 100000      | 10      | 2,883.62 μs | 165.050 μs | 486.653 μs | 2,855.65 μs |      64 B |
| MorpehContext           | 100000      | 10      | 4,500.94 μs | 177.567 μs | 520.772 μs | 4,432.55 μs |     400 B |
| DefaultECSContext       | 100000      | 10      | 4,909.98 μs |  97.741 μs | 178.726 μs | 4,855.00 μs |     736 B |

# SystemWith2Components

| Context                 | EntityCount | Padding |        Mean |      Error |     StdDev |      Median | Allocated |
|-------------------------|-------------|---------|------------:|-----------:|-----------:|------------:|----------:|
| TinyEcsContext          | 100000      | 0       |    28.52 μs |   2.937 μs |   8.040 μs |    27.50 μs |    2032 B |
| ArchContext             | 100000      | 0       |   262.32 μs |   5.080 μs |   6.605 μs |   261.20 μs |     448 B |
| FrifloContext           | 100000      | 0       |   271.66 μs |   5.388 μs |   9.852 μs |   276.60 μs |     184 B |
| LeoEcsContext           | 100000      | 0       |   292.35 μs |   5.415 μs |   8.270 μs |   290.70 μs |     400 B |
| 📍 **StaticEcsContext** | 100000      | 0       |   358.34 μs |   6.910 μs |   6.787 μs |   355.55 μs |     112 B |
| XenoContext             | 100000      | 0       |   415.19 μs |   4.245 μs |   3.545 μs |   414.45 μs |     112 B |
| LeoEcsLiteContext       | 100000      | 0       |   421.87 μs |   2.670 μs |   2.498 μs |   422.30 μs |     400 B |
| FennecsContext          | 100000      | 0       |   452.05 μs |  21.586 μs |  61.586 μs |   445.40 μs |     592 B |
| MassiveEcsContext       | 100000      | 0       |   478.43 μs |   2.494 μs |   2.083 μs |   478.50 μs |     400 B |
| MorpehContext           | 100000      | 0       |   941.87 μs |  12.943 μs |  11.474 μs |   941.10 μs |     400 B |
| FlecsNETContext         | 100000      | 0       | 1,363.78 μs |  25.476 μs |  22.584 μs | 1,365.45 μs |     112 B |
| DragonECSContext        | 100000      | 0       | 1,364.10 μs |   9.759 μs |   8.149 μs | 1,363.60 μs |     112 B |
| DefaultECSContext       | 100000      | 0       | 1,871.29 μs |  29.630 μs |  50.314 μs | 1,858.90 μs |     736 B |
|                         |             |         |             |            |            |             |           |
| TinyEcsContext          | 100000      | 10      |    31.86 μs |   4.350 μs |  12.411 μs |    30.50 μs |    2048 B |
| FrifloContext           | 100000      | 10      |   215.54 μs |   3.748 μs |   7.741 μs |   213.35 μs |     232 B |
| ArchContext             | 100000      | 10      |   265.36 μs |   3.292 μs |   2.749 μs |   265.40 μs |     736 B |
| LeoEcsContext           | 100000      | 10      |   456.87 μs |   8.968 μs |   7.950 μs |   458.05 μs |     400 B |
| XenoContext             | 100000      | 10      |   712.09 μs |  22.492 μs |  65.254 μs |   698.80 μs |     400 B |
| FennecsContext          | 100000      | 10      |   731.88 μs |  77.115 μs | 224.949 μs |   706.70 μs |     880 B |
| 📍 **StaticEcsContext** | 100000      | 10      |   885.12 μs |  17.284 μs |  23.074 μs |   875.50 μs |     400 B |
| MorpehContext           | 100000      | 10      | 1,229.18 μs |  24.233 μs |  48.953 μs | 1,221.95 μs |     400 B |
| DragonECSContext        | 100000      | 10      | 1,345.90 μs | 116.717 μs | 342.310 μs | 1,457.50 μs |     112 B |
| FlecsNETContext         | 100000      | 10      | 1,371.31 μs |  26.882 μs |  30.957 μs | 1,368.55 μs |     112 B |
| MassiveEcsContext       | 100000      | 10      | 1,671.49 μs |  28.117 μs |  24.925 μs | 1,667.00 μs |     400 B |
| LeoEcsLiteContext       | 100000      | 10      | 2,492.29 μs |  36.272 μs |  66.325 μs | 2,474.40 μs |     400 B |
| DefaultECSContext       | 100000      | 10      | 4,975.55 μs |  97.315 μs | 115.847 μs | 4,960.80 μs |     400 B |

# SystemWith3ComponentsMultipleComposition

| Context                 | EntityCount | Padding |        Mean |      Error |     StdDev |      Median | Allocated |
|-------------------------|-------------|---------|------------:|-----------:|-----------:|------------:|----------:|
| TinyEcsContext          | 100000      | 0       |    29.18 μs |   5.876 μs |  17.233 μs |    27.25 μs |    1696 B |
| ArchContext             | 100000      | 0       |   372.51 μs |   7.364 μs |   8.767 μs |   373.70 μs |    1408 B |
| FrifloContext           | 100000      | 0       |   432.14 μs |   4.284 μs |   3.798 μs |   432.70 μs |     520 B |
| 📍 **StaticEcsContext** | 100000      | 0       |   477.15 μs |   3.215 μs |   2.684 μs |   478.20 μs |     400 B |
| XenoContext             | 100000      | 0       |   529.44 μs |   9.885 μs |   8.254 μs |   530.30 μs |     112 B |
| LeoEcsLiteContext       | 100000      | 0       |   546.99 μs |   2.922 μs |   2.590 μs |   546.80 μs |     400 B |
| LeoEcsContext           | 100000      | 0       |   549.91 μs |   8.696 μs |   9.666 μs |   551.80 μs |     400 B |
| FennecsContext          | 100000      | 0       |   582.85 μs |  39.339 μs | 112.236 μs |   571.25 μs |     912 B |
| MassiveEcsContext       | 100000      | 0       |   685.16 μs |   5.234 μs |   4.371 μs |   682.80 μs |     400 B |
| MorpehContext           | 100000      | 0       | 1,506.22 μs |  29.030 μs |  37.748 μs | 1,498.00 μs |     400 B |
| DragonECSContext        | 100000      | 0       | 1,576.59 μs | 204.921 μs | 604.214 μs | 1,291.40 μs |     112 B |
| FlecsNETContext         | 100000      | 0       | 1,857.18 μs |  21.340 μs |  42.618 μs | 1,848.80 μs |     400 B |
| DefaultECSContext       | 100000      | 0       | 7,240.79 μs |  35.797 μs |  31.733 μs | 7,236.00 μs |     736 B |
|                         |             |         |             |            |            |             |           |
| TinyEcsContext          | 100000      | 10      |    15.38 μs |   2.227 μs |   6.318 μs |    12.60 μs |    1696 B |
| ArchContext             | 100000      | 10      |   391.80 μs |   7.262 μs |   6.792 μs |   390.60 μs |    1408 B |
| FrifloContext           | 100000      | 10      |   440.96 μs |   8.677 μs |  18.678 μs |   432.90 μs |     520 B |
| LeoEcsContext           | 100000      | 10      |   584.93 μs |  11.288 μs |  23.312 μs |   579.75 μs |     400 B |
| FennecsContext          | 100000      | 10      |   856.14 μs |  74.096 μs | 216.140 μs |   802.50 μs |     912 B |
| 📍 **StaticEcsContext** | 100000      | 10      |   975.65 μs |  19.357 μs |  41.667 μs |   969.55 μs |     400 B |
| MassiveEcsContext       | 100000      | 10      | 1,373.05 μs |  26.408 μs |  22.052 μs | 1,376.10 μs |     400 B |
| FlecsNETContext         | 100000      | 10      | 2,023.96 μs | 101.657 μs | 296.538 μs | 1,851.80 μs |     400 B |
| DragonECSContext        | 100000      | 10      | 2,150.13 μs |  40.741 μs |  59.718 μs | 2,128.80 μs |     400 B |
| XenoContext             | 100000      | 10      | 2,523.87 μs |  50.221 μs | 138.323 μs | 2,495.00 μs |     400 B |
| LeoEcsLiteContext       | 100000      | 10      | 2,716.03 μs |  35.543 μs |  27.750 μs | 2,703.15 μs |     400 B |
| DefaultECSContext       | 100000      | 10      | 5,335.89 μs |  84.448 μs |  74.861 μs | 5,310.80 μs |     736 B |
| MorpehContext           | 100000      | 10      | 5,496.46 μs | 109.370 μs | 230.699 μs | 5,511.95 μs |     400 B |

# SystemWith3Components

| Context                 | EntityCount | Padding |        Mean |      Error |     StdDev | Allocated |
|-------------------------|-------------|---------|------------:|-----------:|-----------:|----------:|
| TinyEcsContext          | 100000      | 0       |    33.09 μs |   6.532 μs |  19.053 μs |    1984 B |
| ArchContext             | 100000      | 0       |   328.93 μs |   5.730 μs |   4.784 μs |     736 B |
| FrifloContext           | 100000      | 0       |   400.30 μs |   3.495 μs |   3.098 μs |     520 B |
| 📍 **StaticEcsContext** | 100000      | 0       |   477.11 μs |   2.781 μs |   2.465 μs |     400 B |
| XenoContext             | 100000      | 0       |   505.21 μs |   5.754 μs |   4.805 μs |     400 B |
| LeoEcsLiteContext       | 100000      | 0       |   544.96 μs |   2.469 μs |   2.189 μs |      64 B |
| LeoEcsContext           | 100000      | 0       |   548.42 μs |   6.095 μs |   4.758 μs |     400 B |
| FennecsContext          | 100000      | 0       |   587.43 μs |  40.531 μs | 117.587 μs |     880 B |
| MassiveEcsContext       | 100000      | 0       |   689.05 μs |   6.979 μs |   6.187 μs |     112 B |
| MorpehContext           | 100000      | 0       | 1,381.92 μs |  27.490 μs |  31.658 μs |     400 B |
| FlecsNETContext         | 100000      | 0       | 1,824.44 μs |  26.729 μs |  20.868 μs |     400 B |
| DragonECSContext        | 100000      | 0       | 2,105.13 μs |  31.771 μs |  26.530 μs |     400 B |
| DefaultECSContext       | 100000      | 0       | 2,202.42 μs |  22.752 μs |  36.740 μs |     736 B |
|                         |             |         |             |            |            |           |
| TinyEcsContext          | 100000      | 10      |    25.58 μs |   3.703 μs |  10.683 μs |    1984 B |
| ArchContext             | 100000      | 10      |   345.04 μs |   4.896 μs |   8.703 μs |     736 B |
| MassiveEcsContext       | 100000      | 10      |   345.82 μs |   6.843 μs |   5.714 μs |      64 B |
| FrifloContext           | 100000      | 10      |   407.78 μs |   5.897 μs |   9.852 μs |     520 B |
| LeoEcsContext           | 100000      | 10      |   589.52 μs |  10.797 μs |  11.087 μs |     400 B |
| FennecsContext          | 100000      | 10      |   806.91 μs |  82.186 μs | 238.437 μs |     880 B |
| 📍 **StaticEcsContext** | 100000      | 10      |   822.19 μs |  16.033 μs |  41.098 μs |      64 B |
| XenoContext             | 100000      | 10      |   863.09 μs |  17.152 μs |  41.424 μs |     400 B |
| MorpehContext           | 100000      | 10      | 1,599.67 μs |  23.112 μs |  21.619 μs |     400 B |
| FlecsNETContext         | 100000      | 10      | 1,824.57 μs |  25.359 μs |  23.721 μs |     400 B |
| DragonECSContext        | 100000      | 10      | 2,280.14 μs |  23.841 μs |  21.134 μs |     400 B |
| LeoEcsLiteContext       | 100000      | 10      | 3,208.73 μs |  12.265 μs |  10.242 μs |     400 B |
| DefaultECSContext       | 100000      | 10      | 5,397.52 μs | 101.858 μs | 113.215 μs |     736 B |

# ThreeAddOneComponent

| Context                 | EntityCount |        Mean |       Error |      StdDev |      Median |  Allocated |
|-------------------------|-------------|------------:|------------:|------------:|------------:|-----------:|
| 📍 **StaticEcsContext** | 100000      |    405.6 μs |     8.10 μs |     8.67 μs |    403.0 μs |      400 B |
| MassiveEcsContext       | 100000      |    676.5 μs |    35.74 μs |    93.52 μs |    718.6 μs |      400 B |
| DragonECSContext        | 100000      |    900.3 μs |    24.53 μs |    66.73 μs |    896.1 μs |      400 B |
| DefaultECSContext       | 100000      |  1,430.7 μs |    28.54 μs |    31.72 μs |  1,435.2 μs |      400 B |
| LeoEcsLiteContext       | 100000      |  1,815.8 μs |    25.25 μs |    22.38 μs |  1,812.9 μs |      400 B |
| XenoContext             | 100000      |  3,064.9 μs |    54.31 μs |    76.14 μs |  3,047.6 μs |   983696 B |
| MorpehContext           | 100000      |  3,085.1 μs |   112.88 μs |   318.39 μs |  2,957.7 μs |      400 B |
| FrifloContext           | 100000      |  3,115.0 μs |    21.64 μs |    19.18 μs |  3,118.4 μs |       64 B |
| LeoEcsContext           | 100000      |  3,736.3 μs |    61.31 μs |    51.20 μs |  3,723.4 μs |       64 B |
| TinyEcsContext          | 100000      |  4,258.4 μs |    83.15 μs |    92.42 μs |  4,244.0 μs |  2400400 B |
| ArchContext             | 100000      | 12,226.3 μs |    78.53 μs |    65.57 μs | 12,211.6 μs |     1744 B |
| FennecsContext          | 100000      | 64,978.0 μs | 1,299.14 μs | 2,440.10 μs | 64,851.2 μs | 84000400 B |
| FlecsNETContext         | 100000      | 91,826.1 μs | 1,278.72 μs | 1,133.55 μs | 92,152.9 μs |      400 B |

# ThreeRemoveOneComponent

| Context                 | EntityCount |        Mean |       Error |      StdDev |      Median |  Allocated |
|-------------------------|-------------|------------:|------------:|------------:|------------:|-----------:|
| 📍 **StaticEcsContext** | 100000      |    538.5 μs |     4.98 μs |     3.89 μs |    537.6 μs |      400 B |
| MassiveEcsContext       | 100000      |    678.8 μs |     9.14 μs |    18.87 μs |    676.1 μs |      400 B |
| TinyEcsContext          | 100000      |    819.4 μs |    26.82 μs |    72.51 μs |    791.6 μs |       64 B |
| DragonECSContext        | 100000      |  1,021.0 μs |    26.07 μs |    68.68 μs |  1,010.9 μs |      400 B |
| DefaultECSContext       | 100000      |  1,117.0 μs |    15.57 μs |    13.81 μs |  1,112.5 μs |      400 B |
| FrifloContext           | 100000      |  2,353.8 μs |    84.25 μs |   245.77 μs |  2,200.2 μs |      400 B |
| MorpehContext           | 100000      |  2,916.5 μs |    59.15 μs |   172.55 μs |  2,908.8 μs |      400 B |
| LeoEcsLiteContext       | 100000      |  3,085.6 μs |    32.97 μs |    27.54 μs |  3,086.2 μs |      400 B |
| XenoContext             | 100000      |  3,222.0 μs |    42.66 μs |    35.62 μs |  3,222.1 μs |   983400 B |
| LeoEcsContext           | 100000      |  6,949.9 μs |   133.02 μs |   117.92 μs |  6,969.4 μs |      112 B |
| ArchContext             | 100000      | 37,583.5 μs |   731.31 μs |   570.96 μs | 37,655.1 μs | 24804408 B |
| FennecsContext          | 100000      | 53,201.7 μs | 1,061.60 μs | 1,941.19 μs | 52,809.9 μs | 58400400 B |
| FlecsNETContext         | 100000      | 73,114.9 μs |   908.04 μs |   849.38 μs | 73,211.2 μs |      400 B |

# ThreeRemoveTwoComponents

| Context                 | EntityCount |       Mean |     Error |    StdDev |     Median |  Allocated |
|-------------------------|-------------|-----------:|----------:|----------:|-----------:|-----------:|
| 📍 **StaticEcsContext** | 100000      |   1.130 ms | 0.0047 ms | 0.0042 ms |   1.130 ms |      400 B |
| DragonECSContext        | 100000      |   1.795 ms | 0.0457 ms | 0.1195 ms |   1.760 ms |      400 B |
| MassiveEcsContext       | 100000      |   2.095 ms | 0.0414 ms | 0.0892 ms |   2.089 ms |       64 B |
| DefaultECSContext       | 100000      |   2.112 ms | 0.0199 ms | 0.0166 ms |   2.115 ms |      400 B |
| TinyEcsContext          | 100000      |   2.599 ms | 0.0513 ms | 0.0479 ms |   2.588 ms |      400 B |
| MorpehContext           | 100000      |   3.265 ms | 0.0635 ms | 0.0756 ms |   3.274 ms |      400 B |
| XenoContext             | 100000      |   3.610 ms | 0.0611 ms | 0.0970 ms |   3.573 ms |   983352 B |
| FrifloContext           | 100000      |   3.691 ms | 0.0196 ms | 0.0153 ms |   3.688 ms |      400 B |
| LeoEcsLiteContext       | 100000      |   5.742 ms | 0.2227 ms | 0.5944 ms |   5.533 ms |       64 B |
| LeoEcsContext           | 100000      |  10.333 ms | 0.3792 ms | 0.9990 ms |   9.980 ms |      400 B |
| ArchContext             | 100000      |  38.625 ms | 0.6705 ms | 0.7452 ms |  38.427 ms | 24805616 B |
| FennecsContext          | 100000      |  80.129 ms | 1.2988 ms | 1.2149 ms |  79.810 ms | 96800400 B |
| FlecsNETContext         | 100000      | 117.614 ms | 1.2109 ms | 1.1327 ms | 117.936 ms |      400 B |

# TwoAddOneComponent

| Context                 | EntityCount |        Mean |     Error |      StdDev |      Median |  Allocated |
|-------------------------|-------------|------------:|----------:|------------:|------------:|-----------:|
| 📍 **StaticEcsContext** | 100000      |    402.9 μs |   8.02 μs |    17.59 μs |    394.4 μs |      400 B |
| DragonECSContext        | 100000      |    894.2 μs |  19.43 μs |    54.17 μs |    886.2 μs |      400 B |
| MassiveEcsContext       | 100000      |  1,012.1 μs |   6.93 μs |     5.41 μs |  1,010.0 μs |      400 B |
| DefaultECSContext       | 100000      |  1,383.7 μs |  16.00 μs |    13.36 μs |  1,381.8 μs |      400 B |
| LeoEcsLiteContext       | 100000      |  1,705.3 μs |  31.96 μs |    24.95 μs |  1,700.8 μs |      400 B |
| FrifloContext           | 100000      |  2,380.5 μs |  67.02 μs |   183.46 μs |  2,293.5 μs |      400 B |
| XenoContext             | 100000      |  3,067.0 μs |  60.14 μs |   109.97 μs |  3,036.8 μs |   983696 B |
| MorpehContext           | 100000      |  3,088.2 μs |  78.04 μs |   225.16 μs |  3,046.2 μs |      400 B |
| LeoEcsContext           | 100000      |  3,593.4 μs |  37.17 μs |    32.95 μs |  3,596.6 μs |      400 B |
| TinyEcsContext          | 100000      |  4,004.2 μs |  79.75 μs |   176.72 μs |  3,982.3 μs |  2400400 B |
| ArchContext             | 100000      | 10,612.9 μs | 132.48 μs |   110.63 μs | 10,620.7 μs |     1744 B |
| FennecsContext          | 100000      | 52,825.4 μs | 986.53 μs | 1,382.98 μs | 52,124.7 μs | 71200400 B |
| FlecsNETContext         | 100000      | 88,251.8 μs | 534.28 μs |   473.62 μs | 88,186.1 μs |      400 B |

# TwoAddTwoComponents

| Context                 | EntityCount |         Mean |       Error |      StdDev |   Allocated |
|-------------------------|-------------|-------------:|------------:|------------:|------------:|
| 📍 **StaticEcsContext** | 100000      |     726.1 μs |    12.05 μs |    10.68 μs |       400 B |
| DragonECSContext        | 100000      |   1,431.1 μs |    28.56 μs |    53.65 μs |       400 B |
| MassiveEcsContext       | 100000      |   1,440.5 μs |    19.21 μs |    17.03 μs |       400 B |
| DefaultECSContext       | 100000      |   2,688.3 μs |    46.57 μs |    41.28 μs |       400 B |
| MorpehContext           | 100000      |   3,560.7 μs |    75.94 μs |   215.42 μs |       400 B |
| XenoContext             | 100000      |   3,631.1 μs |    59.08 μs |    49.33 μs |    983696 B |
| LeoEcsLiteContext       | 100000      |   3,819.6 μs |    69.14 μs |    79.62 μs |       400 B |
| FrifloContext           | 100000      |   4,734.9 μs |    10.79 μs |     8.42 μs |       400 B |
| LeoEcsContext           | 100000      |   6,282.3 μs |    56.24 μs |    43.91 μs |       112 B |
| TinyEcsContext          | 100000      |   7,101.2 μs |   203.52 μs |   528.98 μs |   4800400 B |
| ArchContext             | 100000      |  12,446.9 μs |   162.16 μs |   135.41 μs |      1456 B |
| FennecsContext          | 100000      | 116,758.2 μs | 2,299.30 μs | 3,777.82 μs | 155200400 B |
| FlecsNETContext         | 100000      | 190,912.0 μs | 1,986.51 μs | 1,858.18 μs |       400 B |

# TwoRemoveOneComponent

| Context                 | EntityCount |        Mean |     Error |      StdDev |      Median |  Allocated |
|-------------------------|-------------|------------:|----------:|------------:|------------:|-----------:|
| 📍 **StaticEcsContext** | 100000      |    532.8 μs |   3.96 μs |     3.51 μs |    532.1 μs |      400 B |
| MassiveEcsContext       | 100000      |    674.5 μs |  12.43 μs |    26.22 μs |    667.4 μs |      400 B |
| TinyEcsContext          | 100000      |    892.8 μs |  17.52 μs |    15.53 μs |    896.6 μs |      400 B |
| DragonECSContext        | 100000      |    986.3 μs |  19.68 μs |    43.61 μs |    972.6 μs |      400 B |
| DefaultECSContext       | 100000      |  1,130.0 μs |  18.00 μs |    15.95 μs |  1,127.7 μs |      400 B |
| FrifloContext           | 100000      |  1,911.4 μs |  27.74 μs |    29.69 μs |  1,903.2 μs |      400 B |
| LeoEcsLiteContext       | 100000      |  2,295.9 μs |  29.10 μs |    24.30 μs |  2,295.2 μs |      400 B |
| MorpehContext           | 100000      |  2,994.8 μs |  77.69 μs |   215.28 μs |  2,920.9 μs |       64 B |
| XenoContext             | 100000      |  3,126.4 μs |  46.45 μs |    75.00 μs |  3,107.5 μs |   983352 B |
| LeoEcsContext           | 100000      |  6,011.2 μs | 114.84 μs |   136.71 μs |  5,983.1 μs |      400 B |
| ArchContext             | 100000      | 32,056.2 μs | 473.44 μs |   395.34 μs | 32,075.3 μs | 24005248 B |
| FennecsContext          | 100000      | 36,663.9 μs | 726.65 μs | 1,152.54 μs | 36,240.5 μs | 44000400 B |
| FlecsNETContext         | 100000      | 67,200.7 μs | 639.96 μs |   598.62 μs | 67,264.7 μs |      400 B |

