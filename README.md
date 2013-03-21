# FSharpMandelbrot
Benchmarks the generation of a Mandelbrot fractal.

## Configuration
Since this was to be a simple project, configuration is done by adjusting
constants and recompiling.

## Parallel vs Serial Benchmarking
To change this program into a serial Mandelbrot generator, change
    Array.Parallel.init
to
    Array.init
and the generation should take several times longer, depending on how many cores
you have available for processing.
