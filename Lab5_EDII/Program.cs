using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using System.Linq;

public class Person
{
    public string Name { get; set; }
    public string DPI { get; set; }
    public string DateOfBirth { get; set; }
    public string Address { get; set; }
    public List<string> Companies { get; set; }
    public string Recluiter { get; set; }

    public string ClientPublicRSAKey { get; set; }
    public string ClientPrivateRSAKey { get; set; }
    public string RecruiterPublicRSAKey { get; set; }
    public string RecruiterPrivateRSAKey { get; set; }

    // Generar claves RSA para el cliente y el reclutador
    public void GenerateRSAKeys()
    {
        using (RSACryptoServiceProvider clientRSA = new RSACryptoServiceProvider(1024))
        using (RSACryptoServiceProvider recruiterRSA = new RSACryptoServiceProvider(1024))
        {
            // Generar claves para el cliente
            ClientPublicRSAKey = clientRSA.ToXmlString(false);
            ClientPrivateRSAKey = clientRSA.ToXmlString(true);

            // Generar claves para el reclutador
            RecruiterPublicRSAKey = recruiterRSA.ToXmlString(false);
            RecruiterPrivateRSAKey = recruiterRSA.ToXmlString(true);
        }
    }

}

public class Node
{
    public Person Data { get; set; }
    public Node Left { get; set; }
    public Node Right { get; set; }

    public Node(Person person)
    {
        Data = person;
        Left = null;
        Right = null;
    }
}

public class BinaryTree
{
    private Node root;
    private List<Person> personsList;

    public BinaryTree()
    {
        root = null;
        personsList = new List<Person>();
    }

    public void Insert(Person person)
    {
        root = InsertRec(root, person);
        if (root != null)
        {
            personsList.Add(person);

            // Generar las claves RSA para la nueva persona
            person.GenerateRSAKeys();
        }
    }

    private Node InsertRec(Node root, Person person)
    {
        if (root == null)
        {
            root = new Node(person);
            Console.WriteLine("Inserción exitosa: " + person.Name);
        }
        else
        {
            int compareResult = string.Compare(person.Name, root.Data.Name, StringComparison.OrdinalIgnoreCase);
            if (compareResult < 0)
            {
                root.Left = InsertRec(root.Left, person);
            }
            else if (compareResult > 0)
            {
                root.Right = InsertRec(root.Right, person);
            }
            else
            {
                // La persona ya existe en el árbol, actualiza la lista de compañías
                root.Data.Companies = person.Companies; // Actualiza la lista de compañías
                Console.WriteLine("Persona con el mismo nombre ya existe: " + person.Name);
            }
        }
        return root;
    }

    public void Update(string name, Person updatedPerson)
    {
        root = UpdateRec(root, name, updatedPerson);
    }

    private Node UpdateRec(Node root, string name, Person updatedPerson)
    {
        if (root == null)
        {
            Console.WriteLine("No se encontró la persona a actualizar: " + name);
        }
        else
        {
            int compareResult = string.Compare(name, root.Data.Name, StringComparison.OrdinalIgnoreCase);
            if (compareResult == 0)
            {
                root.Data = updatedPerson;
                Console.WriteLine("Actualización exitosa para: " + name);
            }
            else if (compareResult < 0)
            {
                root.Left = UpdateRec(root.Left, name, updatedPerson);
            }
            else
            {
                root.Right = UpdateRec(root.Right, name, updatedPerson);
            }
        }
        return root;
    }
    public void InOrderTraversal()
    {
        InOrderTraversalRec(root);
    }

    private void InOrderTraversalRec(Node root)
    {
        if (root != null)
        {
            InOrderTraversalRec(root.Left);
            Console.WriteLine(root.Data);
            InOrderTraversalRec(root.Right);
        }
    }
    public void Delete(string nameToDelete)
    {
        root = DeleteRec(root, nameToDelete);
        if (root != null)
        {
            personsList.RemoveAll(p => p.Name.Equals(nameToDelete, StringComparison.OrdinalIgnoreCase));
        }
    }

    private Node DeleteRec(Node root, string nameToDelete)
    {
        if (root == null)
        {
            // No se encontró la persona a eliminar
            Console.WriteLine("No se encontró la persona a eliminar: " + nameToDelete);
            return root;
        }

        int compareResult = string.Compare(nameToDelete, root.Data.Name, StringComparison.OrdinalIgnoreCase);
        if (compareResult < 0)
        {
            root.Left = DeleteRec(root.Left, nameToDelete);
        }
        else if (compareResult > 0)
        {
            root.Right = DeleteRec(root.Right, nameToDelete);
        }
        else
        {
            // Se encontró la persona a eliminar
            Console.WriteLine("Eliminación exitosa: " + nameToDelete);

            // Caso 1: No tiene hijos o solo un hijo
            if (root.Left == null)
            {
                return root.Right;
            }
            else if (root.Right == null)
            {
                return root.Left;
            }

            // Caso 2: Tiene dos hijos, se encuentra el sucesor inmediato
            root.Data = FindMinValue(root.Right);

            // Elimina el sucesor inmediato
            root.Right = DeleteRec(root.Right, root.Data.Name);
        }
        return root;
    }
    private Person FindMinValue(Node node)
    {
        Person minValue = node.Data;
        while (node.Left != null)
        {
            minValue = node.Left.Data;
            node = node.Left;
        }
        return minValue;
    }
    //Busqueda de datos
    public Person Search(string name)
    {
        return SearchRec(root, name);
    }

    private Person SearchRec(Node root, string name)
    {
        if (root == null)
        {
            // No se encontró la persona
            return null;
        }

        int compareResult = string.Compare(name, root.Data.Name, StringComparison.OrdinalIgnoreCase);
        if (compareResult == 0)
        {
            // Se encontró la persona
            return root.Data;
        }
        else if (compareResult < 0)
        {
            // La persona podría estar en el subárbol izquierdo
            return SearchRec(root.Left, name);
        }
        else
        {
            // La persona podría estar en el subárbol derecho
            return SearchRec(root.Right, name);
        }
    }
}

public class BitacoraEntry
{
    public string Name { get; set; }
    public string DPI { get; set; }
    public string DateOfBirth { get; set; }
    public string Address { get; set; }
    public List<string> Companies { get; set; }

    public BitacoraEntry(Person person)
    {
        Name = person.Name;
        DPI = person.DPI;
        DateOfBirth = person.DateOfBirth;
        Address = person.Address;

        Companies = new List<string>();
        if (person.Companies != null)
        {
            Companies.AddRange(person.Companies);
        }
    }
}

class Program
{
    
    static string encryptionKey = "1234512345123456";
    static string folderPath = @"D:\Cosas\Clases\2023\Estructura de datos II\txt\Mensajes";

    static void Main()
    {

        List<Person> allPersons = new List<Person>();
        Dictionary<string, List<Person>> peopleByName = new Dictionary<string, List<Person>>();

        string inputFilePath = @"D:\Cosas\Clases\2023\Estructura de datos II\txt\Datos.csv"; // Ruta del archivo CSV

        // Obtener la carpeta donde se encuentra el archivo CSV
        string csvFolder = Path.GetDirectoryName(folderPath);

        // Construir la ruta completa para el archivo de bitácora en la misma carpeta
        string bitacoraFilePath = Path.Combine(csvFolder, "bitacora.txt");


        // Convierte la clave en una matriz de bytes
        byte[] keyBytes = Encoding.UTF8.GetBytes(encryptionKey);

        HashSet<string> allCompanies = new HashSet<string>();
        HashSet<string> allRecruiters = new HashSet<string>();

        if (File.Exists(inputFilePath))
        {
            List<string> lines = ReadCsvFile(inputFilePath);
            foreach (string line in lines)
            {
                string[] parts = line.Split(';');
                if (parts.Length == 2)
                {
                    string action = parts[0].Trim();
                    string data = parts[1].Trim();
                    switch (action)
                    {
                        case "INSERT":
                            {
                                var personData = JsonConvert.DeserializeObject<Person>(data);
                                personData.GenerateRSAKeys(); // Genera las claves RSA para la persona
                                allPersons.Add(personData);

                                // Verificar si ya existe una lista de personas con este nombre
                                if (!peopleByName.ContainsKey(personData.Name))
                                {
                                    peopleByName[personData.Name] = new List<Person>();
                                }

                                if (personData.Companies != null)
                                {
                                    allCompanies.UnionWith(personData.Companies);
                                }

                                if (!string.IsNullOrEmpty(personData.Recluiter))
                                {
                                    allRecruiters.Add(personData.Recluiter);
                                }

                                peopleByName[personData.Name].Add(personData);
                                break;
                            }
                        case "PATCH":
                            var updatedPersonData = JsonConvert.DeserializeObject<Person>(data);
                            var personToUpdate = allPersons.FirstOrDefault(p => p.Name == updatedPersonData.Name);
                            if (personToUpdate != null)
                            {
                                // Actualiza la persona en la lista
                                personToUpdate.DPI = updatedPersonData.DPI;
                                personToUpdate.DateOfBirth = updatedPersonData.DateOfBirth;
                                personToUpdate.Address = updatedPersonData.Address;
                                personToUpdate.Companies = updatedPersonData.Companies;

                                if (personToUpdate.Companies != null)
                                {
                                    allCompanies.UnionWith(personToUpdate.Companies);
                                }
                                if (!string.IsNullOrEmpty(personToUpdate.Recluiter))
                                {
                                    allRecruiters.Add(personToUpdate.Recluiter);
                                }

                            }
                            else
                            {
                                Console.WriteLine("No se encontró la persona para actualizar.");
                            }
                            break;
                        case "DELETE":
                            var deleteData = JsonConvert.DeserializeObject<Person>(data);
                            if (peopleByName.ContainsKey(deleteData.Name))
                            {
                                // Eliminar la primera persona con ese nombre
                                peopleByName[deleteData.Name].RemoveAt(0);
                            }
                            else
                            {
                                Console.WriteLine("No se encontró la persona para eliminar.");
                            }
                            break;

                        default:
                            Console.WriteLine("Acción no reconocida: " + action);
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Formato incorrecto en línea: " + line);
                }
            }
            Console.WriteLine("Datos cargados correctamente desde CSV.");
        }
        else
        {
            Console.WriteLine("El archivo CSV no existe en la ubicación especificada.");
        }

        string rutaMensajes = @"D:\Cosas\Clases\2023\Estructura de datos II\txt\Mensajes";

        // Recorre los archivos en la carpeta "rutaMensajes" y cifra cada archivo con la clave pública del reclutador.
        foreach (string archivo in Directory.GetFiles(rutaMensajes, "*.txt"))
        {
            string contenido = File.ReadAllText(archivo);
            string nombreArchivo = Path.GetFileNameWithoutExtension(archivo);

            // Comprobar si el archivo comienza con "REC-" o "CONV-".
            if (nombreArchivo.StartsWith("REC-") || nombreArchivo.StartsWith("CONV-"))
            {
                // Extraer el DPI del nombre del archivo (por ejemplo, "REC-9949882398116-1.txt").
                string[] partes = nombreArchivo.Split('-');
                if (partes.Length >= 2)
                {
                    string dpi = partes[1];

                    // Buscar la clave pública del reclutador en la lista "personList" utilizando el DPI.
                    Person reclutador = allPersons.FirstOrDefault(p => p.DPI == dpi);
                    if (reclutador != null)
                    {
                        // Generar una clave simétrica para cifrar el mensaje.
                        byte[] symmetricKey = GenerateAESKey();
                        byte[] iv = GenerateAESIV();

                        string contenidoCifrado = EncryptWithAES(contenido, symmetricKey, iv);

                        // Cifrar la clave simétrica con la clave pública del reclutador.
                        string claveSimetricaCifrada = EncryptSymmetricKeyWithRSA(symmetricKey, reclutador.RecruiterPublicRSAKey);

                        // Sobrescribir el archivo original con la clave simétrica cifrada y el mensaje cifrado.
                        File.WriteAllText(archivo, claveSimetricaCifrada + "\n" + iv + "\n" + contenidoCifrado);
                    }
                }
            }
        }



        Console.Clear();
        Console.WriteLine("----------------------------------");
        Console.WriteLine("Sistema de validación:            ");
        Console.WriteLine("----------------------------------");
        Console.WriteLine("\nRecruiters:");
        int count = 1;
        foreach (var recruiter in allRecruiters)
        {
            Console.WriteLine(count + ". " + recruiter);
            count++;

        }
        Console.WriteLine("\nQuien eres:");
        string Reclutador = Console.ReadLine();

        // Genera la contraseña del reclutador según el patrón
        string nombreReclutador = Reclutador.Length >= 3 ? Reclutador.Substring(0, 3) : Reclutador;
        string contraseñaReclutadorGenerada = $"{nombreReclutador}2023huB";

        Console.Clear();
        Console.WriteLine("----------------------------------");
        Console.WriteLine("Sistema de validación:            ");
        Console.WriteLine("----------------------------------");
        Console.WriteLine("Compañías:");
        count = 1;
        foreach (var company in allCompanies)
        {
            Console.WriteLine(count + ". " + company);
            count++;
        }
        Console.WriteLine("\nDe que compañia eres:");
        string Compañia = Console.ReadLine();

        bool tieneRelacion = false;

        foreach (var personsWithSameName in peopleByName.Values)
        {
            foreach (var person in personsWithSameName)
            {
                if (person.Recluiter == Reclutador && person.Companies.Contains(Compañia))
                {
                    tieneRelacion = true;
                    break; // Puedes salir del ciclo una vez que se encuentre una relación válida.
                }
            }
        }

        bool contraseñaValida = false;

        // Solicitar la contraseña basada en el patrón
        Console.Clear();
        Console.WriteLine("Ingresa tu contraseña:");
        Console.WriteLine("----------------------------------");
        string Contraseña = Console.ReadLine();

        if (Contraseña == contraseñaReclutadorGenerada)
        {
            // La contraseña es válida
            contraseñaValida = true;
        }
        else
        {
            Console.WriteLine("La contraseña ingresada no coincide con la generada.");
            Console.ReadKey();
        }

        if (tieneRelacion && contraseñaValida)
        {
            // Crear una lista para almacenar la información de las personas a mostrar
            List<string> personasParaMostrar = new List<string>();
            Console.WriteLine("El reclutador y la compañía seleccionados están relacionados.");
            Console.ReadKey();
            while (true)
            {

                Console.Clear();
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("Uusuario: " + Reclutador);
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("Menú:");
                Console.WriteLine("1. Ver listado de personas");
                Console.WriteLine("2. Ver claves publicas");
                Console.WriteLine("3. Mensajes cifrados");
                Console.WriteLine("X. Salir");

                Console.Write("Selecciona una opción: ");
                string option = Console.ReadLine();
                Console.Clear();
                if (option.Equals("1", StringComparison.OrdinalIgnoreCase))
                {
                    // Opción 1: Ver listado de personas relacionadas a un reclutador específico
                    Console.Clear();

                    string reclutadorSeleccionado = Reclutador;

                    Console.Clear();
                    Console.WriteLine($"Listado de Personas relacionadas al reclutador: {reclutadorSeleccionado} y a la compañía: {Compañia}");

                 

                    foreach (var pair in peopleByName)
                    {
                        foreach (var person in pair.Value)
                        {
                            if (person.Recluiter == reclutadorSeleccionado && person.Companies.Contains(Compañia))
                            {
                                string personaInfo = $"Nombre: {person.Name} - DPI: {person.DPI}";
                                Console.WriteLine(personaInfo);
                                personasParaMostrar.Add(personaInfo);
                            }
                        }
                    }

                    Console.WriteLine("Presiona cualquier tecla para continuar...");
                    Console.ReadKey();
                }
                if (option.Equals("2", StringComparison.OrdinalIgnoreCase))
                {
                    Console.Clear();
                    Console.WriteLine("Ingresa el DPI de la persona cuya información deseas ver:");
                    string dpiToSearch = Console.ReadLine();
                    bool found = false;

                    // Encuentra la persona con el DPI ingresado
                    Person targetPerson = null;
                    foreach (var personList in peopleByName.Values)
                    {
                        foreach (var person in personList)
                        {
                            if (person.DPI.Equals(dpiToSearch, StringComparison.OrdinalIgnoreCase))
                            {
                                targetPerson = person;
                                found = true;
                                break;
                            }
                        }
                        if (found)
                        {
                            break;
                        }
                    }

                    if (found && targetPerson != null)
                    {
                        Console.Clear();
                        Console.WriteLine("-------------------------------");
                        Console.WriteLine("Persona encontrada:");
                        Console.WriteLine("-------------------------------");
                        Console.WriteLine("Nombre: " + targetPerson.Name);
                        Console.WriteLine("DPI: " + targetPerson.DPI);
                        Console.WriteLine("Clave Pública:");
                        Console.WriteLine(targetPerson.ClientPublicRSAKey);
                    }
                    else
                    {
                        Console.WriteLine("No se encontró ninguna persona con ese DPI.");
                    }

                    Console.WriteLine("Presiona cualquier tecla para continuar...");
                    Console.ReadKey();
                }
                else if (option.Equals("3", StringComparison.OrdinalIgnoreCase))
                {
                    // Opción 3: Buscar DPI y ver mensajes relacionados
                    Console.Clear();
                    Console.WriteLine("Ingresa el DPI a buscar:");
                    string dpiToSearch = Console.ReadLine();
                    bool found = false;

                    // Encuentra la persona con el DPI ingresado
                    Person targetPerson = null;
                    foreach (var personList in peopleByName.Values)
                    {
                        foreach (var person in personList)
                        {
                            if (person.DPI.Equals(dpiToSearch, StringComparison.OrdinalIgnoreCase))
                            {
                                targetPerson = person;
                                found = true;
                                break;
                            }
                        }
                        if (found)
                        {
                            break;
                        }
                    }

                    if (found && targetPerson != null)
                    {
                        Console.Clear();
                        Console.WriteLine("-------------------------------");
                        Console.WriteLine("Persona encontrada:");
                        Console.WriteLine("-------------------------------");
                        Console.WriteLine("Nombre: " + targetPerson.Name);
                        Console.WriteLine("DPI: " + targetPerson.DPI);
                        Console.WriteLine("-------------------------------");
                        Console.WriteLine();
                        Console.ReadKey();

                        // Menú para opciones relacionadas con el DPI
                        while (true)
                        {
                            Console.Clear();
                            Console.WriteLine();
                            Console.WriteLine("---------------------------------------");
                            Console.WriteLine("Nombre: " + targetPerson.Name);
                            Console.WriteLine("DPI: " + targetPerson.DPI);
                            Console.WriteLine("---------------------------------------");
                            Console.WriteLine("Menú de opciones relacionadas al DPI:");
                            Console.WriteLine("1. Mostrar mensajes relacionados al DPI");
                            Console.WriteLine("2. Ver mensaje cifrado");
                            Console.WriteLine("3. Descifrar mensaje");
                            Console.WriteLine("4. Regresar al menú inicial");

                            Console.Write("Selecciona una opción: ");
                            string dpiOption = Console.ReadLine();
                            Console.Clear();
                            Console.WriteLine();
                            Console.WriteLine("---------------------------------------");

                            if (dpiOption.Equals("1", StringComparison.OrdinalIgnoreCase))
                            {
                                string dpiToSearch2 = targetPerson.DPI;
                                string directoryPath = @"D:\Cosas\Clases\2023\Estructura de datos II\txt\Mensajes";

                                // Encuentra todos los archivos que contienen el DPI en el nombre
                                string[] relatedMessages = Directory.GetFiles(directoryPath, $"*-{dpiToSearch2}-*.txt");

                                Console.WriteLine($"Número de mensajes relacionados: {relatedMessages.Length}");

                                // Lista los nombres de los mensajes relacionados
                                Console.WriteLine("Nombres de mensajes relacionados:");
                                foreach (string messageFile in relatedMessages)
                                {
                                    string messageName = Path.GetFileName(messageFile);
                                    Console.WriteLine(messageName);
                                }
                            }
                            else if (dpiOption.Equals("2", StringComparison.OrdinalIgnoreCase))
                            {
                                Console.Write("Ingresa el prefijo (CONV o REC): ");
                                string prefijo = Console.ReadLine();
                                Console.Write("Ingresa el número de carta: ");
                                string numeroCarta = Console.ReadLine();

                                string filePrefix = prefijo.ToUpper() == "CONV" ? "CONV-" : "REC-";

                                // Genera el nombre del archivo correspondiente
                                string messageFilename = $"{filePrefix}{targetPerson.DPI}-{numeroCarta}.txt";

                                // Directorio donde se encuentran los archivos de cartas de recomendación o mensajes
                                string directoryPath = @"D:\Cosas\Clases\2023\Estructura de datos II\txt\Mensajes";

                                // Ruta completa del archivo
                                string messageFilePath = Path.Combine(directoryPath, messageFilename);

                                // Verifica si el archivo existe
                                if (File.Exists(messageFilePath))
                                {
                                    // Lee el contenido del archivo
                                    string messageContent = File.ReadAllText(messageFilePath);

                                    // Muestra el mensaje en la consola
                                    Console.WriteLine("---------------------------------------");
                                    Console.WriteLine("Contenido del mensaje:");
                                    Console.WriteLine("");
                                    Console.WriteLine("> " + messageFilename);
                                    Console.WriteLine("");
                                    Console.WriteLine(messageContent);
                                }
                                else
                                {
                                    Console.WriteLine("El archivo especificado no existe.");
                                }
                            }
                            else if (dpiOption.Equals("3", StringComparison.OrdinalIgnoreCase))
                            {
                                Console.Write("Ingresa el prefijo (CONV o REC): ");
                                string prefijo = Console.ReadLine();
                                Console.Write("Ingresa el número de carta: ");
                                string numeroCarta = Console.ReadLine();

                                string filePrefix = prefijo.ToUpper() == "CONV" ? "CONV-" : "REC-";
                                string archivoADescifrar = $"{filePrefix}{targetPerson.DPI}-{numeroCarta}.txt";

                                // Directorio donde se encuentran los archivos de mensajes cifrados
                                string directoryPath = @"D:\Cosas\Clases\2023\Estructura de datos II\txt\Mensajes";

                                // Ruta completa del archivo cifrado
                                string messageFilePath = Path.Combine(directoryPath, archivoADescifrar);

                                // Verifica si el archivo existe
                                if (File.Exists(messageFilePath))
                                {
                                    string mensajeDescifrado = DescifrarMensaje(messageFilePath, targetPerson.RecruiterPrivateRSAKey);

                                    if (!string.IsNullOrEmpty(mensajeDescifrado))
                                    {
                                        Console.WriteLine("---------------------------------------");
                                        Console.WriteLine("Mensaje descifrado:");
                                        Console.WriteLine("");
                                        Console.WriteLine(mensajeDescifrado);
                                    }
                                    else
                                    {
                                        Console.WriteLine("No se pudo descifrar el mensaje. Verifica la clave privada o el archivo.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("El archivo especificado no existe.");
                                }
                            }
                            else if (dpiOption.Equals("4", StringComparison.OrdinalIgnoreCase))
                            {
                                // Opción 3: Regresar al menú inicial
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Opción no válida.");
                            }
                            Console.ReadKey();
                        }
                    }
                    else
                    {
                        Console.WriteLine("No se encontró ninguna persona con ese DPI.");
                    }


                    Console.WriteLine("Presiona cualquier tecla para continuar...");
                    Console.ReadKey();


                }
                else if (option.Equals("X", StringComparison.OrdinalIgnoreCase))
                {
                    break; // Salir del programa
                }
                else
                {
                    Console.WriteLine("Opción no válida. Presiona cualquier tecla para continuar...");
                }
            }

        }
        else
        {
            Console.WriteLine("El reclutador y la compañía seleccionados no están relacionados.");
            Console.ReadKey();
        }
       

        
    }


    // Método para leer datos desde un archivo CSV y devolverlos como una lista de cadenas
    static List<string> ReadCsvFile(string filePath)
    {
        List<string> lines = new List<string>();

        using (TextFieldParser parser = new TextFieldParser(filePath))
        {
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(";");

            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields();
                if (fields != null)
                {
                    string line = string.Join(";", fields);
                    lines.Add(line);
                }
            }
        }

        return lines;
    }
    public static byte[] GenerateAESKey()
    {
        using (RijndaelManaged rijAlg = new RijndaelManaged())
        {
            rijAlg.GenerateKey();
            return rijAlg.Key;
        }
    }

    // Función para generar un vector de inicialización (IV) para AES.
    public static byte[] GenerateAESIV()
    {
        using (RijndaelManaged rijAlg = new RijndaelManaged())
        {
            rijAlg.GenerateIV();
            return rijAlg.IV;
        }
    }

    // Función para cifrar un mensaje con AES.
    public static string EncryptWithAES(string mensaje, byte[] symmetricKey, byte[] iv)
    {
        using (RijndaelManaged rijAlg = new RijndaelManaged())
        {
            rijAlg.Key = symmetricKey;
            rijAlg.IV = iv;

            byte[] mensajeBytes = Encoding.UTF8.GetBytes(mensaje);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, rijAlg.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    csEncrypt.Write(mensajeBytes, 0, mensajeBytes.Length);
                    csEncrypt.FlushFinalBlock();
                }

                byte[] mensajeCifradoBytes = msEncrypt.ToArray();
                string mensajeCifrado = Convert.ToBase64String(mensajeCifradoBytes);
                return mensajeCifrado;
            }
        }
    }

    // Función para cifrar la clave simétrica con RSA.
    public static string EncryptSymmetricKeyWithRSA(byte[] symmetricKey, string publicKey)
    {
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            try
            {
                rsa.FromXmlString(publicKey);
                byte[] symmetricKeyCiphertext = rsa.Encrypt(symmetricKey, false);
                string symmetricKeyCiphertextBase64 = Convert.ToBase64String(symmetricKeyCiphertext);
                return symmetricKeyCiphertextBase64;
            }
            finally
            {
                rsa.PersistKeyInCsp = false;
            }
        }
    }

    static string DescifrarMensaje(string filePath, string privateKey)
    {
        try
        {
            // Lee el contenido cifrado del archivo
            string mensajeCifrado = File.ReadAllText(filePath);

            // Divide el contenido en las partes (clave simétrica cifrada, IV y contenido cifrado)
            string[] partes = mensajeCifrado.Split('\n');

            if (partes.Length == 3)
            {
                string claveSimetricaCifradaEnBase64 = partes[0];
                string ivEnBase64 = partes[1];
                string contenidoCifradoEnBase64 = partes[2];

                // Convierte las partes de Base64 a bytes
                byte[] claveSimetricaCifradaBytes = Convert.FromBase64String(claveSimetricaCifradaEnBase64);
                byte[] ivBytes = Convert.FromBase64String(ivEnBase64);
                byte[] contenidoCifradoBytes = Convert.FromBase64String(contenidoCifradoEnBase64);

                // Crea una instancia de RSACryptoServiceProvider y carga la clave privada
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    rsa.FromXmlString(privateKey);

                    // Descifra la clave simétrica utilizando la clave privada
                    byte[] claveSimetricaBytes = rsa.Decrypt(claveSimetricaCifradaBytes, false);

                    // Descifra el contenido utilizando la clave simétrica y IV
                    string mensajeDescifrado = DecryptWithAES(contenidoCifradoBytes, claveSimetricaBytes, ivBytes);

                    return mensajeDescifrado;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error al descifrar el mensaje: " + e.Message);
        }

        return string.Empty; // Retorna una cadena vacía en caso de error
    }


    public static string DecryptSymmetricKeyWithRSA(byte[] symmetricKeyCiphertext, string privateKey)
    {
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            try
            {
                rsa.FromXmlString(privateKey);
                byte[] symmetricKeyBytes = rsa.Decrypt(symmetricKeyCiphertext, false);
                return Convert.ToBase64String(symmetricKeyBytes);
            }
            finally
            {
                rsa.PersistKeyInCsp = false;
            }
        }
    }

    public static string DecryptWithAES(byte[] mensajeCifradoBytes, byte[] symmetricKey, byte[] iv)
    {
        using (RijndaelManaged rijAlg = new RijndaelManaged())
        {
            rijAlg.Key = symmetricKey;
            rijAlg.IV = iv;

            using (MemoryStream msDecrypt = new MemoryStream(mensajeCifradoBytes))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, rijAlg.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }

}

