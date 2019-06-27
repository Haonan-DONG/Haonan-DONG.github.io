% calculate the file.
clc,clear
fid=fopen('txt.txt','r');
n1=3;
a=[];

for i=1:n1
    tmp=str2num(fgetl(fid));
    a=[a;tmp];
end

ri=[0,0,0.58,0.90,1.12];

[x,y]=eig(a);
lambda=max(diag(y));
num=find(diag(y)==lambda);
w0=x(:,num)/sum(x(:,num));
cr0=(lambda-n1)/(n1-1)/ri(n1)