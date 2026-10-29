# WestSide Roleplay - RAGE:MP

Gamemode de roleplay pentru **GTA V**, dezvoltat în C# pentru platforma **RAGE:MP**.

> [!WARNING]
> **Proiect discontinuat.** Dezvoltarea și mentenanța au fost oprite după închiderea RAGE:MP. Repository-ul este păstrat în scop de arhivă și ca referință; nu există suport activ sau garanția că poate fi rulat în prezent.

## Funcționalități

- **Conturi și autentificare** — înregistrare, autentificare și asocierea contului cu jucătorul.
- **Personaje** — creare și selectare de personaje, date și atribute de caracter, precum și personalizare pentru haine, păr și tatuaje.
- **Facțiuni** — facțiuni, membri, invitații și ranguri.
- **Inventar și obiecte** — gestionarea inventarului, folosirea și echiparea obiectelor, arme și transferul obiectelor între jucători.
- **Telefoane** — numere de telefon și mesaje SMS.
- **Vehicule** — vehicule personale, salvarea poziției, chei, motor și faruri.
- **Case și interioare** — intrare și ieșire din case și gestionarea accesului.
- **Magazine** — magazine configurabile cu obiecte și un modul pentru benzinării.
- **Interfață și experiența jucătorului** — HUD pentru bani, locație și vehicul, blip-uri, etichete de nume și interacțiuni cu interfața CEF.
- **Administrare și instrumente** — comenzi administrative, loguri și mesaje de chat.

## Tehnologii

- C# și **.NET Core 3.1**
- **RAGE:MP** și API-ul C# `rage-sharpN`
- **MySQL** pentru persistența datelor
- **Newtonsoft.Json** pentru serializarea datelor
- **CEF** pentru unele interfețe client

## Structura repository-ului

- [`source/client`](source/client) — codul sursă pentru client.
- [`source/server`](source/server) — codul sursă pentru server.
- [`server-files/client_packages/cs_packages/ClientSide`](server-files/client_packages/cs_packages/ClientSide) — proiectul client inclus în fișierele serverului.
- [`server-files/dotnet/resources/ServerSide`](server-files/dotnet/resources/ServerSide) — proiectul server inclus în fișierele serverului.

Repository-ul păstrează codul sursă și resursele necesare interfețelor, dar **nu include executabile, biblioteci compilate sau output-uri de build**. Soluția Visual Studio este [`GM DE RP .NET CORE 3.1.sln`](GM%20DE%20RP%20.NET%20CORE%203.1.sln). Compilarea necesită SDK-ul .NET Core 3.1 și dependențele externe RAGE:MP, inclusiv `rage-sharpN`; acestea nu sunt incluse, iar disponibilitatea și funcționarea lor pe versiunile actuale nu sunt garantate.
