git clone http://llvm.org/git/llvm.git
git -C llvm reset --hard 9bde5fb8f8ea44d6ef0f2edb4f54cbff7a0aee53
cd llvm\tools
git clone http://llvm.org/git/clang.git
git -C clang reset --hard ffc5930a5cffb23df03565739b8820d14a19d4bf
cd ..\..
premake5 --file=scripts/LLVM.lua --arch=x64 --debug build_llvm
rem premake5 --file=scripts/LLVM.lua --arch=x64 --debug package_llvm
