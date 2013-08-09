# copy Runtime

src=~/exdev/ex2d/ex2d_dev/Assets/ex2D/Runtime
dest=~/exdev/ex2d_doc/Runtime

rm -rf $dest
mkdir -p $dest

cd $src
find . -type d -name '*' -exec mkdir -p $dest/{} \;
find . -type f -iname "*.cs" -exec cp {} $dest/{} \;
find . -type f -iname "*.shader" -exec cp {} $dest/{} \;
cd $dest
