using AppForSEII2526.API.DTOs.ReseñasDTOs; 

namespace AppForSEII2526.Web
{
    public class ReviewStateContainer
    {
        // Almacén principal de datos (Sesión de reseña)
        public ReviewDTO CurrentReview { get; private set; } = new ReviewDTO();

        // Propiedad calculada: Solo habilita el botón "Reseñar" si hay items
        public bool CanStartReview => CurrentReview.ReviewItems.Any();

        // Evento para notificar a la UI 
        public event Action? OnChange;
        private void NotifyStateChanged() => OnChange?.Invoke();

       
        // Lógica de Selección (El Puente)
        

        // Recibe el objeto de la lista (DevicesReseñaDTO)
        public void AddDeviceToReview(DevicesReseñaDTO deviceFromCatalog)
        {
            // 1. Evitar duplicados: Comprobamos por ID
            if (!CurrentReview.ReviewItems.Any(ri => ri.DeviceId == deviceFromCatalog.Id))
            {
                // 2. Mapeo (Transformación):
                // Convertimos el DTO de visualización en un DTO de reseña
                var newItem = new ReviewItemDTO()
                {
                    // IDs y claves
                    DeviceId = deviceFromCatalog.Id,

                    // Datos informativos (copiamos lo que viene del catálogo)
                    // Nota: Mapeamos 'Brand' a 'DeviceName' para que coincida con tu DTO anterior
                    DeviceName = deviceFromCatalog.Brand,
                    DeviceModel = deviceFromCatalog.Model,
                    DeviceYear = deviceFromCatalog.Year,

                    // Inicializamos los campos vacíos para que el usuario escriba luego
                    Rating = 0,
                    Comment = string.Empty
                };

                // 3. Añadir a la sesión
                CurrentReview.ReviewItems.Add(newItem);

                // 4. Actualizar vista
                NotifyStateChanged();
            }
        }

        
        // Métodos de Gestión (Eliminar, Limpiar)
        public void RemoveDevice(int deviceId)
        {
            var item = CurrentReview.ReviewItems.FirstOrDefault(i => i.DeviceId == deviceId);
            if (item != null)
            {
                CurrentReview.ReviewItems.Remove(item);
                NotifyStateChanged();
            }
        }

        // Se llama al finalizar el Paso 6 o si el usuario cancela
        public void RestartSession()
        {
            CurrentReview = new ReviewDTO();
            NotifyStateChanged();
        }
    }
}