# StanaGO

StanaGO este o platforma web dedicata digitalizarii comertului mioritic. Aplicatia conecteaza producatorii locali cu turistii, facilitand comertul cu produse traditionale si sporind siguranta stanelor prin alerte in timp real.

## Functionalitati Cheie

* **Harta Interactiva:** Vizualizarea stanelor si a produselor folosind Leaflet.js și OpenStreetMap.
* **Sistem de Alerte:** Raportarea si afisarea pericolelor (ursi, lupi) în timp real, cu filtrare pe baza distanței.
* **Chat:** Sistem de mesagerie privata.
* **Gestiune Produse:** Ciobanii pot administra inventarul, prețurile și disponibilitatea produselor.
* **Permisiuni pe Roluri:** Acces diferentiat pentru utilizatori inregistrati și ciobani.
* **Interfata Responsive:** Design adaptabil pentru dispozitive mobile.

## Tehnologii Utilizate

* **Framework:** ASP.NET Core MVC
* **Limbaj:** C#
* **Baza de date:** Microsoft SQL Server
* **ORM:** Entity Framework Core
* **Frontend:** Razor Pages, Bootstrap 5, JavaScript
* **Harti:** Leaflet.js


## Instructiuni de Instalare

1.  Clonati repository-ul:
    git clone https://github.com/username/StanaGO.git

2.  Configurati conexiunea la baza de date in fisierul `appsettings.json` (sectiunea ConnectionStrings).

3.  Aplicati migratiile pentru a crea baza de date:
    dotnet ef database update

4.  Rulati aplicatia:
    dotnet run

## Echipa

* Popescu Florian
* Todi Tinu-Constantin
