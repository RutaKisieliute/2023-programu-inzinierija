#!/bin/bash -x
cd $(dirname $0)

dotnet dev-certs https
sudo -E dotnet dev-certs https -ep /usr/local/share/ca-certificates/aspnet/https.crt --format PEM
sudo update-ca-certificates
mkdir -p $HOME/.pki/nssdb
sudo certutil -d sql:$HOME/.pki/nssdb -A -t "P,," -n localhost -i /usr/local/share/ca-certificates/aspnet/https.crt
sudo certutil -d sql:$HOME/.pki/nssdb -A -t "C,," -n localhost -i /usr/local/share/ca-certificates/aspnet/https.crt

dotnet run --project ./AALKisMVCUI/AALKisMVCUI.csproj --configuration Debug

dotnet dev-certs https --clean
sudo rm -r /usr/local/share/ca-certificates/aspnet
sudo update-ca-certificates
sudo certutil -d sql:$HOME/.pki/nssdb -D -n localhost
