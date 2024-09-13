e:

REM build Neptune programs
cd \Code\Waketree\Waketree.Neptune.Compiler\bin\Debug\net7.0
cd Samples
del *.compiled
cd ..
Waketree.Neptune.Compiler.exe Samples\HelloWorld.neptune Samples\HelloWorld.neptune.compiled
Waketree.Neptune.Compiler.exe Samples\ThreadTest.neptune Samples\ThreadTest.neptune.compiled
Waketree.Neptune.Compiler.exe Samples\ThreadTestMutex.neptune Samples\ThreadTestMutex.neptune.compiled

REM copy compiled files to local runtime
del \Code\Waketree\Waketree.Neptune.Runtime.Local\bin\Debug\net7.0\*.compiled
cd \Code\Waketree\Waketree.Neptune.Compiler\bin\Debug\net7.0\Samples
copy *.compiled \Code\Waketree\Waketree.Neptune.Runtime.Local\bin\Debug\net7.0\.
copy *.compiled \Code\Waketree\Waketree.Service\bin\Debug\net7.0\.
cd \Code\Waketree