Leap Motion - Unity 3D Pattern Recognition
=============

Simple Unity example showing how to detect gesture patterns by straightening the point collection 
from the leap frames, using the Ramer-Douglas-Peucker algorithm, and then comparing the sharp 
turns made during the gesture.

For more information on the Ramer-Douglas-Peucker algorithm, check 
http://en.wikipedia.org/wiki/Ramer-Douglas-Peucker_algorithm

The examples shows the path that the Leap Motion device recognized and the points being considered
when detecting the pattern made. The small squares are points still considered after a high precision
passing of the Ramer-Douglas-Peucker algorithm, and the big squares are points considered after a
lower precision pass on the algorithm, which is done if no gestures were detected with the first.

Gestures this example is able to detect:

![Downwards Zig-Zag Gesture](https://raw.github.com/DaniloT/leap_patterns/master/Assets/Textures/gesture_downzig.jpg "Downwards Zig-Zag")  
![Rightwards Zig-Zag Gesture](https://raw.github.com/DaniloT/leap_patterns/master/Assets/Textures/gesture_rightzig.jpg "Rightwards Zig-Zag")  
![Square Gesture](https://raw.github.com/DaniloT/leap_patterns/master/Assets/Textures/gesture_square.jpg "Square")



License info:

The MIT License (MIT)
Copyright (c) 2013 Danilo Gaby Andersen Trindade

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
