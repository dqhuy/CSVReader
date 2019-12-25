% Ten ham chinh phai trung voi ten file
function [msg] = analysis(rootpath) 

%% Clustering
% Grey clustering model

filename = strcat(rootpath,'\DataInput\input_industry.csv');
[ndata, text, alldata] = xlsread(filename,1); % Doc du lieu cac chi so
y = ndata;
[Ni,Nc] = size(y);
Ni = Ni-1; % Ni - Number of industries, Nc = Number of criteria

% Define the possibility functions fjk the jth criterion of the kth class
% k = 1 - General, k = 2 - Aux, k = 3 - Leading
filename =  strcat(rootpath,'\DataInput\input_posibility.csv');
[ndata, text, alldata] = xlsread(filename);
for k = 2:Nc+1
    f = char(text(2,k));
    f1(k-1,:) = str2num(f);
    f = char(text(3,k));
    f2(k-1,:) = str2num(f);
    f = char(text(4,k));
    f3(k-1,:) = str2num(f);
end
% Weight
filename =  strcat(rootpath,'\DataInput\input_weight.csv');
[ndata, text, alldata] = xlsread(filename);
nuy = ndata(2,:);
% Calculate Clustering Coefficients
sigma = zeros(Ni,3);
for m = 1:Ni
    for k = 1:Nc
        sigma(m,1)=sigma(m,1)+fjkfun(f1(k,:),y(m+1,k))*nuy(k);
        sigma(m,2)=sigma(m,2)+fjkfun(f2(k,:),y(m+1,k))*nuy(k);
        sigma(m,3)=sigma(m,3)+fjkfun(f3(k,:),y(m+1,k))*nuy(k);
    end
end
[MaxSig,Class]=max(sigma,[],2);
kqi = [sigma MaxSig Class];
filename =  strcat(rootpath,'\DataOutput\KQ_industry.csv'); % Ghi ra file kq industries
csvwrite(filename,kqi,1,1);

%% Optimization
% Tien xu ly du lieu

filename =  strcat(rootpath,'\DataInput\input_gdp.csv');
[ndata, text, alldata] = xlsread(filename); % Doc du lieu tu file excel
year = ndata(:,1);
GDP = ndata(:,2:end);
% [ndata, text, alldata] = xlsread(filename,2); % Doc du lieu cac nguon luc
filename =  strcat(rootpath,'\DataInput\input_hr.csv');
[ndata, text, alldata] = xlsread(filename); % Doc du lieu tu file excel
HR = ndata(:,2:end);  % Employees
filename =  strcat(rootpath,'\DataInput\input_cap.csv');
[ndata, text, alldata] = xlsread(filename); % Doc du lieu tu file excel
CAP = ndata(:,2:end);  % Investment
filename =  strcat(rootpath,'\DataInput\input_energy.csv');
[ndata, text, alldata] = xlsread(filename); % Doc du lieu tu file excel
ENG = ndata(:,2:end); % Energy consumption
filename =  strcat(rootpath,'\DataInput\input_water.csv');
[ndata, text, alldata] = xlsread(filename); % Doc du lieu tu file excel
WAT = ndata(:,2:end);  % Water consumption
ndata = [year HR CAP ENG WAT];
K = 4; % So luong nguon luc
Ni = size(GDP,2); % So nganh cong nghiep
for k = 0:K-1
    bi = ndata(:,k+2*(k+1):k+2*(k+1)+Ni-1);
    b(:,k+1) = sum(bi,2); % Tinh cac luong tai nguyen
    a(:,k+2*(k+1)-1:k+2*(k+1)+Ni-2) = bi./GDP; % Tinh cac he so dong gop theo GDP
end
a(:,1:3) = a(:,1:3)*1e4;

filename =  strcat(rootpath,'\DataOutput\KQ_A.csv');
csvwrite(filename,b,1,1);  % Luu ket qua tinh he so b 
filename = strcat(rootpath,'\DataOutput\KQ_B.csv');
csvwrite(filename,a,1,1);  % Luu ket qua tinh he so a

% Du doan theo mo hinh GM(1,1)
% Prediction
% [ndata, text, alldata] = xlsread(filename,3); % Doc du lieu tu file excel
% y = ndata(:,2:end);
y = [b a];
Np = 7; % So diem du doan tiep theo
N = size(y,2);
for k = 1:N;
    [pdata(:,k),err(k)] = gm11(y(:,k),Np);
end
pdata(:,14) = 0;
pb = pdata(:,1:4);
filename = strcat(rootpath,'\DataOutput\DB_B.csv');
csvwrite(filename,pb,1,1);  % Luu ket qua du bao he so b 
pa = pdata(:,5:end);
filename = strcat(rootpath,'\DataOutput\DB_A.csv');
csvwrite(filename,pa,1,1);  % Luu ket qua du bao he so a 

%% Structure optimization of 3 major industries (x1, x2, x3)
%  f = x1 + x2 + x3
% [ndata, text, alldata] = xlsread(filename,4); % Doc du lieu duoc du doan
% y = ndata(:,2:end);
y = [pb pa];
Ny = size(y,1);
f = [-1; -1; -1];
for k = 1:Ny
    A =  [y(k,5:7);...  1
          y(k,8:10);...
          y(k,11:13);...
          y(k,14:16)];
    b = [y(k,1)*1e4; y(k,2); y(k,3); y(k,4)];
    lb = zeros(Ni,1);
    [x,fval,exitflag,output,lanmbda] = linprog(f,A,b,[],[],lb);
    xi(k,:) = x;
    Z(k,:) = -fval; 
    
end
pgdp = [xi Z];
filename = strcat(rootpath,'\DataOutput\DB_GDP.csv');
csvwrite(filename,pgdp,1,1);  % Luu ket qua du bao GDP
msg='Done';
end


function [pdata,err] = gm11(y,N)
% GM(1,1) model to predict the data
% y=input('please enter the data:');
n=length(y);yy=ones(n,1);yy(1)=y(1);
for i=2:n
    yy(i)=yy(i-1)+y(i);  % AGO
end
B=ones(n-1,2);
for i=1:(n-1)
    B(i,1)=-(yy(i)+yy(i+1))/2;B(i,2)=1;
end
BT=B';
for j=1:n-1
    YN(j)=y(j+1);
end
YN=YN';
A = inv(BT*B)*BT*YN;  % Computing a & b parameters
a=A(1);u=A(2);t=u/a;
i=1:N+n;
yys(i+1)=(y(1)-t).*exp(-a.*i)+t;
yys(1)=y(1);
for j=n+N:-1:2
    ys(j)=yys(j)-yys(j-1);  % IAGO
end
% x=1:n;
% xs=2:n+N;
yn=ys(2:n+N);
% plot(x,y,'^r',xs,yn,'*-b');
pdata = ys(n+1:n+N);
det=0;
for i=2:n
    det=det+abs((yn(i)-y(i))/y(i));  % Relative Error
end
det=det/(n-1);
err = det;

end

function fjk_val = fjkfun(f,x)
% Calculate fjk coeff.
a = isnan(f);
if a == 0
    if x <f(1)||x>f(4)
        fjk_val = 0;
    elseif x >=f(1)&&x<f(2)
        fjk_val = (x-f(1))/(f(2)-f(1));
    elseif x >=f(2)&&x<f(3)
        fjk_val = 1;
    else
        fjk_val = (f(4)-x)/(f(4)-f(3));
    end
elseif (a(1) == 1&&a(2) == 1)
    if x <0||x>f(4)
        fjk_val = 0;
    elseif x >=0&&x<f(3)
        fjk_val = 1;
    else
        fjk_val = (f(4)-x)/(f(4)-f(3));
    end
elseif (a(3) == 1&&a(4) == 1)
    if x <f(1)
        fjk_val = 0;
    elseif x >=f(1)&&x<f(2)
        fjk_val = (x-f(1))/(f(2)-f(1));
    else
        fjk_val = 1;
    end
else
    if x <f(1)||x>f(4)
        fjk_val = 0;
    elseif x >=f(1)&&x<f(2)
        fjk_val = (x-f(1))/(f(2)-f(1));
    else
        fjk_val = (f(4)-x)/(f(4)-f(2));
    end
end
    
end