FROM microsoft/dotnet:1.1.4-runtime
WORKDIR /root
COPY bin/Release/netcoreapp1.1/publish /root
COPY DeviceCount.json /root
ENTRYPOINT ["dotnet", "MockDataCreate.dll"]
