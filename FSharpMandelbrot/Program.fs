open System
open System.Diagnostics
open System.Drawing
open System.Windows.Forms
open System.Numerics
open Microsoft.FSharp.Math

//#########################################
//## Mandelbrot functions
//#########################################

// Recursively performs the transform Z -> Z^2 + c for the given number of
// iterations or until we escape the Z > 2 threshold
let rec transform (originalNum:Complex) (currentNum:Complex) maxIterations iterationNumber =
    if (iterationNumber = maxIterations) then 0
    else if (currentNum.Magnitude > 2.0) then iterationNumber
    else
        let sqr = Complex.Multiply(currentNum, currentNum)
        let newNum = Complex.Add(sqr, originalNum)
        let newIter = iterationNumber + 1
        transform originalNum newNum maxIterations newIter

// Calculate whether that point is in the set, to the best of our ability
// given maxIterations.  Since we are on the real/imaginary plane, the
// coordinates are given as a complex number.
let calculateCoordinate (c:Complex) (maxIterations:int) =
    transform c c maxIterations 1


//#########################################
//## Window size and coordinate handling
//#########################################

// Size of window and canvas
let xSize = 1024
let ySize = 720
let ratio = xSize / ySize

// Fine-tuned control over positioning
let xCenter = -0.05
let yCenter = 0.0
let scale = 1.0

// Min and max complex coordinates to display on the canvas
let xMin = xCenter - 2.0 * scale
let xMax = xCenter + 0.8 * scale
let yMin = yCenter - 1.2 * scale
let yMax = yCenter + 1.2 * scale

// Size of each horizontal and vertical pixel in terms of complex numbers
let xFactor = (xMax - xMin) / double(xSize)
let yFactor = (yMax - yMin) / double(ySize)

// Transforms pixel coordinates into corresponding complex numbers
let complexCoordinates(x,y) =
    new Complex((x * xFactor + xMin), (y * yFactor + yMin))


//#########################################
//## UI Logic
//#########################################

// Maps distance from the mandelbrot set to a color
let determineColor (distance:int) =
    if (distance = 0) then Color.Black          // Black if in the set
    else match (distance % 8) with              // Colored if not
        | 1 -> Color.FromArgb(0x66, 0x99, 0xff) // Rotate through similar colors
        | 2 -> Color.FromArgb(0x99, 0x66, 0xff)
        | 3 -> Color.FromArgb(0x99, 0x66, 0xcc)
        | 4 -> Color.FromArgb(0x66, 0x66, 0xcc)
        | 5 -> Color.FromArgb(0x66, 0x66, 0xff)
        | 6 -> Color.FromArgb(0x99, 0x99, 0xff)
        | 7 -> Color.FromArgb(0x99, 0x99, 0xcc)
        | _ -> Color.FromArgb(0x66, 0x99, 0xcc)

// Sets up the canvas and maps the mandelbrot set onto it
let generateMandelbrotImage (width:int) (height:int) (iterations:int) =
    let bmp = new Bitmap(width, height)

    // Calculate distances
    let mandelbrotDistance = Array.Parallel.init width (fun x ->
        Array.init height (fun y ->
            let c = complexCoordinates(double(x), double(y))
            calculateCoordinate c iterations))

    // Set pixels
    for x in 0..(width - 1) do
        for y in 0..(height - 1) do
            let pixelColor = determineColor mandelbrotDistance.[x].[y]
            bmp.SetPixel(x, y, pixelColor)

    // Return the finished image
    bmp


//#########################################
//## Main
//#########################################

[<STAThread>]
do
    // Start the stopwatch
    Console.WriteLine("Generating Mandelbrot...")
    let stopwatch = new Stopwatch()
    stopwatch.Start()

    // Generate the picture
    let box = new PictureBox(BackColor = Color.White,
                             Dock = DockStyle.Fill,
                             SizeMode = PictureBoxSizeMode.CenterImage)
    box.Image <- generateMandelbrotImage xSize ySize 600

    // Stop the stopwatch
    stopwatch.Stop()
    Console.Write("Elapsed time: ")
    Console.Write(stopwatch.ElapsedMilliseconds)
    Console.WriteLine(" ms")

    // Display the picture in a new form
    let mainForm = new Form(Width = xSize, Height = ySize, Text = "Mandelbrot Set")
    mainForm.Controls.Add(box)
    Application.Run(mainForm)

