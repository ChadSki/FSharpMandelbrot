# FSharpMandelbrot
Benchmarks the generation of a Mandelbrot fractal.

## Configuration
Since this was to be a simple project, configuration is done by adjusting
constants and recompiling.

## Parallel vs Serial Benchmarking
To change this program into a serial Mandelbrot generator, find
`Array.Parallel.init` on line 81 of Program.fs and change it to to `Array.init`.
The fractal generation should take several times longer, depending on how many
cores you have available for processing.
