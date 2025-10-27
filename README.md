# 🧠 BlogMVC — Plataforma de Blogging con IA

**BlogMVC** es una plataforma de blogging desarrollada en **ASP.NET Core 9 (MVC + Blazor)** que integra **servicios de inteligencia artificial (IA)** mediante **OpenAI API**.  
El proyecto permite publicar entradas de blog con **análisis automático de sentimientos**, **generación de texto e imágenes** basadas en IA, y tareas recurrentes que enriquecen el contenido.

---

## 🚀 Características principales

- 📝 **Gestión completa de entradas de blog** (crear, editar, eliminar, listar).  
- 👤 **Sistema de usuarios y autenticación** con **ASP.NET Identity**.  
- 💬 **Análisis de sentimientos automático** para comentarios y posts usando **OpenAI**.  
- 🤖 **Generación de texto inteligente** para redactar o mejorar entradas con IA.  
- 🖼️ **Generación de imágenes mediante IA** para ilustrar artículos del blog.  
- 🔄 **Tareas de análisis recurrente** (background jobs) ejecutadas automáticamente.  
- 🗃️ **Base de datos SQL Server local** con migraciones automáticas.  
- ⚙️ **Inyección de dependencias**, **configuraciones por entorno** y **validación de opciones**.  
- 🌐 **Arquitectura MVC** con integración de **Blazor Server** para componentes interactivos.

---

## 🧩 Arquitectura y patrones de diseño

El proyecto sigue una **arquitectura en capas** con principios de **separación de responsabilidades**:

| Capa | Descripción |
|------|--------------|
| **BlogMVC.Entidades** | Define las entidades principales del dominio (Usuario, Entrada, Comentario, etc.). |
| **BlogMVC.Datos** | Gestiona el acceso a datos mediante **Entity Framework Core**. |
| **BlogMVC.Servicios** | Contiene la lógica de negocio y los servicios de IA (chat, imágenes, sentimientos). |
| **BlogMVC.Configuraciones** | Define las clases de configuración y las opciones para OpenAI. |
| **BlogMVC.Utilidades** | Incluye utilidades generales y componentes de soporte. |
| **BlogMVC.Jobs** | Contiene los procesos en segundo plano (análisis de sentimientos recurrente). |

### 🧱 Patrones aplicados

- **MVC (Model-View-Controller)**: arquitectura base del proyecto.  
- **Dependency Injection (DI)**: manejo de servicios y dependencias configurado en `Program.cs`.  
- **Repository & Unit of Work (implícitos via EF Core)**: para persistencia y transacciones.  
- **Options Pattern**: para configurar y validar las claves de OpenAI.  
- **Background Service Pattern**: implementado en `AnalisisSentimientosRecurrente`.  

---

## 🧠 Integración de Inteligencia Artificial

El proyecto usa **OpenAI API** mediante el cliente oficial `OpenAIClient`:

- **ServicioChatOpenAI** → generación de texto o redacción de entradas.  
- **ServicioImagenesOpenAI** → generación de imágenes asociadas a las publicaciones.  
- **AnalisisSentimientosOpenAI** → análisis de sentimientos sobre comentarios y textos del blog.  
- **AnalisisSentimientosRecurrente** → tarea en segundo plano que reevalúa publicaciones de forma periódica.  

Toda la configuración de IA se gestiona a través de la clase `ConfiguracionesIA`, vinculada a `appsettings.json`.

---

## ⚙️ Requisitos previos

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download)  
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) o LocalDB  
- Clave API de [OpenAI](https://platform.openai.com/)  

--- 

## 🛠️ Instalación y ejecución

1. **Clonar el repositorio:**
```bash
git clone https://github.com/Andrein99/BlogMVC.git
cd BlogMVC
```

2. **Instalar dependencias:**
  ```bash
  dotnet restore
  ```
3. **Configurar base de datos:**
  ```bash
  dotnet ef database update
  ```

4. **Configurar secretos de usuario:**
  ```bash
    dotnet user-secrets init
    dotnet user-secrets set "ConfiguracionesIA:modeloTexto" "gpt-4o-mini"
    dotnet user-secrets set "ConfiguracionesIA:modeloImagenes" "dall-e-3"
    dotnet user-secrets set "ConfiguracionesIA:modeloSentimientos" "gpt-4o-mini"
    dotnet user-secrets set "ConfiguracionesIA:llaveOpenAI" "TU_API_KEY_DE_OPENAI"
  ```
  O modificar el archivo de secretos de usuario con la estructura
  ```json
  {
    "ConfiguracionesIA": {
      "modeloTexto": "gpt-4o-mini",
      "modeloImagenes": "dall-e-3",
      "modeloSentimientos": "gpt-4o-mini",
      "llaveOpenAI": "TU_API_KEY_DE_OPENAI"
    }
  }
  ```
Eligiendo el modelo que se quiera utilizar, y empleando la API key de OpenAI propia.

5. **Ejecutar aplicación:**
  ```bash
    dotnet run
  ```
