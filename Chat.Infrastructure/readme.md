``` bash
dotnet ef migrations add initial --context AppIdentityDbContext  -o "IdentityData\Migrations"
dotnet ef database update --context AppIdentityDbContext
```

```bash
dotnet ef migrations add initial --context ChatDbContext  -o "ChatData\Migrations"
dotnet ef database update --context ChatDbContext
```


