namespace Planazo.Modelos
{
    /// <summary>
    /// Representa un usuario del sistema que realiza reservas (Reto adicional 2).
    /// Aplica el principio de encapsulamiento mediante propiedades con validación en constructores.
    /// </summary>
    public class Usuario
    {
        public int Id { get; private set; }
        public string Nombre { get; private set; }
        public string Documento { get; private set; }
        public string Email { get; private set; }
        public string Telefono { get; private set; }

        public Usuario(int id, string nombre, string documento, string email, string telefono)
        {
            if (id <= 0)
                throw new ArgumentException("El ID del usuario debe ser mayor a 0.", nameof(id));

            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre del usuario no puede estar vacío.", nameof(nombre));

            if (string.IsNullOrWhiteSpace(documento))
                throw new ArgumentException("El documento no puede estar vacío.", nameof(documento));

            Id = id;
            Nombre = nombre.Trim();
            Documento = documento.Trim();
            Email = string.IsNullOrWhiteSpace(email) ? "no-registrado@planazo.com" : email.Trim();
            Telefono = string.IsNullOrWhiteSpace(telefono) ? "N/A" : telefono.Trim();
        }

        public override string ToString()
        {
            return $"[{Id}] {Nombre} (Doc: {Documento} | Email: {Email} | Tel: {Telefono})";
        }
    }
}
