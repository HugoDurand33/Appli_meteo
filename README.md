# Appli_meteo

commandes pour faire run ce programme :

```powershell
cd .\Meteo\

dotnet run
```

Pour tester le code avec votre clé API changez la valeur apikey dans la fonction getAPikey() en bas du [fichier](Meteo/Views/MainWindow.axaml.cs)

ou créer un fichier config.json dans le dossier Meteo avec cette syntaxe.

```
{ 
    "APIKey": "mettre_clé_API_ici"
}
```