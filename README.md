# UnityGCTests
A group of unit tests for checking if some methods are **"GC Free"**

## How to install

Method 1 : Download the whole git contents into your **Assets/Editor** directory;

Method 2 : Assume your project is git-ed, then you can use add this git as a submodule under **Assets/Editor** directory;

## How to use

* Open the *Test Runner* located at Unity editor's (Window/Test Runner) Menu
* Run the GCTests
* All unit tests with "OK" prefix means the method used in the test are **"GC Free"**, those with "BAD" prefix are not
* All unit tests are supposed to pass, if any of them fails, it means the original GC results have changed
* You're welcome to add new unit tests :D

![GCTests](https://TMPxyz.github.io/images/GCTests.jpg)

