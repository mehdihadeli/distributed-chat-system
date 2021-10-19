``` bash
dotnet ef migrations add initial --context ApplicationDataContext  -o "Data\Migrations"
dotnet ef database update --context ApplicationDataContext
```
