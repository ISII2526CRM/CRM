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
            if (!CurrentReview.ReviewItems.Any(ri => ri.DeviceId == deviceFromCatalog.Id))
            {
                var newItem = new ReviewItemDTO()
                {
                    DeviceId = deviceFromCatalog.Id,
                    DeviceName = deviceFromCatalog.Name,
                    DeviceModel = deviceFromCatalog.Model,
                    DeviceYear = deviceFromCatalog.Year,

                    Rating = 0,
                    Comment = string.Empty
                };

                CurrentReview.ReviewItems.Add(newItem);
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