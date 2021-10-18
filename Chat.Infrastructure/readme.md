``` bash
dotnet ef migrations add initial --context IdentityDataContext  -o "IdentityData\Migrations"
dotnet ef database update --context IdentityDataContext
```

```bash
dotnet ef migrations add initial --context ChatDataContext  -o "ChatData\Migrations"
dotnet ef database update --context ChatDataContext
```


