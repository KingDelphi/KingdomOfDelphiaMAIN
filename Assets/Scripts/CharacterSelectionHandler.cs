using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CharacterSelectionHandler : MonoBehaviour
{
    public static CharacterSelectionHandler Instance { get; private set; }
    private Dictionary<string, GameObject> characterImages = new Dictionary<string, GameObject>();
    private Dictionary<int, GameObject> characterShadows = new Dictionary<int, GameObject>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeCharacterReferences();
    }

    private void InitializeCharacterReferences()
    {
        Transform canvasTransform = GameObject.Find("CharacterSelectorCanvas")?.transform;
        if (canvasTransform == null)
        {
            Debug.LogError("CharacterSelectorCanvas no encontrado.");
            return;
        }

        foreach (Transform child in canvasTransform)
        {
            if (child.name.StartsWith("Character ("))
            {
                if (int.TryParse(child.name.Replace("Character (", "").Replace(")", ""), out int id))
                {
                    characterShadows[id] = child.gameObject;
                }
            }
            else
            {
                characterImages[child.name] = child.gameObject;
            }
        }
    }

    public void HandlePermaDeath(string characterName)
    {
        this.gameObject.SetActive(true);
        // Desactivar la imagen del personaje
        if (characterImages.TryGetValue(characterName, out GameObject characterImage))
        {
            characterImage.SetActive(false);
        }

        // Buscar un índice correspondiente (ejemplo: si Cerebrito es el #124)
        int shadowID = GetShadowID(characterName);
        if (characterShadows.TryGetValue(shadowID, out GameObject shadow))
        {
            Image shadowImage = shadow.GetComponent<Image>();
            if (shadowImage != null)
            {
                shadowImage.color = Color.red;
            }
        }

        Debug.Log($"PermaDeath aplicado a {characterName} (Sombra {shadowID})");
    }

    private int GetShadowID(string characterName)
    {
        // Aquí se debe definir la lógica para asignar IDs (Ejemplo: Cerebrito -> 124)
        Dictionary<string, int> characterToShadowMap = new Dictionary<string, int>
        {
            { "Cerebrito", 124 },
            { "Esqueletico", 125 },
            { "Lobin", 126 },
            { "La Momia Verdulera", 127 },
            { "Pium Pium", 128 },
            { "El Soldadito Mojoncio", 129 },
            { "El Soldado del Futuro", 130 },
            { "Metamami", 131 },
            { "The Cop", 132 },
            { "El Ninja de la Oscuridad", 133 },
            { "El Soldado Bigotes", 134 },
            { "El Clerigo de la Cruz", 135 },
            { "El Guardia de Acero", 136 },
            { "Tres Deseos", 137 },
            { "La Flecha Helada", 138 },
            { "La Lumbre", 139 },
            { "La Flecha Esmeralda", 140 },
            { "El Pinshi Indio", 141 },
            { "El Tronco Verde", 142 },
            { "El Druida de la Tierra", 143 },
            { "El Mago de Agua", 144 },
            { "El Mago de Fuego", 145 },
            { "El Hechicero de Hielo", 146 },
            { "El Hechicero de la Naturaleza", 147 },
            { "El Hechicero de Luz", 148 },
            { "El Capitan Zoom Zoom", 149 },
            { "Rebeca, la Heroina", 150 },
            { "Chiquitiboom Bombita", 151 },
            { "El General Calavera", 152 },
            { "El General Comando", 153 },
            { "Guardia de la Noche", 154 },
            { "Miguel, el Rey Magico", 155 },
            { "El Caballero de la Cruz", 156 },
            { "El Caballero Carmin", 157 },
            { "El Faraon Escarabajo", 158 },
            { "Estrellita Boom Boom", 159 },
            { "Rimbombante", 160 },
            { "Rakata", 161 },
            { "Paka Pum Pim", 162 },
            { "El Patriota", 163 },
            { "El Samurai Rojo", 164 },
            { "Mushogun, el Samurai Demonio", 165 },
            { "La Reina Floripondia", 166 },
            { "General Arturus", 167 },
            { "Lola, la Coneja", 168 },
            { "La Princesa Mocosa", 169 },
            { "El Verdugo del Infierno", 170 },
            { "El Demonio Purpura", 171 },
            { "Chistosito", 172 },
            { "Bomboncito", 173 },
            { "Gaia, el Caballero Feroz", 174 },
            { "El Rey Cielo", 175 },
            { "El Guerrero de los Ojos Rojos", 176 },
            { "El Guerrero del Fuego Purpura", 177 },
            { "El Guerrero de la Luna Roja", 178 },
            { "El Rey Fantasma", 179 },
            { "El Rey Calavera", 180 },
            { "El Faraon Perro", 181 },
            { "Tutankabron", 182 },
            { "Lowsty, el Laser", 183 },
            { "Giovanni, el Ojo Rojo", 184 },
            { "Espopotanjam", 185 },
            { "El Silenciador", 186 },
            { "Parangaricutirimicuaro", 187 },
            { "Menganito", 188 },
            { "Zutanito", 189 },
            { "Fulanito", 190 },
            { "Elsa Yayin", 191 },
            { "Chiquilin", 192 },
            { "Aguijon del Desierto", 193 },
            { "Tolyn, la Pluma", 194 },
            { "El Inquisidor", 195 },
            { "El Tremendo Capitan Jurgensito", 196 },
            { "Dieguito, el Payasito", 197 },
            { "El Dieguito", 198 },
            { "Jessica, la Moñuda", 199 },
            { "La Reina Adriana", 200 },
            { "Diego, el Rey Destructor", 201 },
            { "Nekril, el Amigo del Conocido", 202 },
            { "Jurgentzu, el Noble", 203 },
            { "Emeth, el Duende Travieso", 204 },
            { "El Rey Jurgentzu", 205 },
            { "Sinkia, el Monstruo de Luz", 206 },
            { "El Diego", 207 },
            { "Jurgentzu, el Leon de Fuego", 208 },
            { "Gerardo, el Rey Dinosaurio", 209 },
            { "Delphi, Guardian Universal", 210 },
            { "Gerardo, el Angel Guardian", 211 },
            { "Delphi, Valkyria Fantasma", 212 },
            { "Delphi, el Mitologico", 213 },
            { "Jurgensaurio, el Dragon Indomable", 214 },
            { "Miriam Delphos", 215 },
            { "Delphi, el Supremo", 216 },
            { "Haken Inferno, el Apocaliptico", 217 },
            { "Delphi, el Rey de los Mundos", 218 },
            { "Martina Delphos", 219 },
            { "Delphi, Rey del Tiempo", 220 },
            { "Chispitas", 221 },
            { "Delphi, el Metamago Creador de Mundos", 222 },
            { "Delphina, la Emperatriz Fenix", 223 },
            { "Delphita, la Pinga", 224 },
            { "Luis, el Angel 666", 225 },
            { "Tao, el Generalissimo Delphi", 226 },
            { "Rayfusha, el Caballero de las Gemas", 227 },
            { "Delphi, el Liberador", 228 },
            { "Darth Ante, el Caballero Dorado", 229 },
            { "Delphi, Rey de Delphia", 230 },
            { "Delphitzu, el Amo de las Travesuras", 231 },
            { "El Destroyer", 232 },
            { "Alianza Delphi", 233 },
            { "Delphi, el Fascinante", 234 },
            { "Delphi, el Magnifico", 235 },
            { "Delphi, el Magnanimo", 236 },
            { "Delphi, la Bestia Dorada", 237 },
            { "La Quimera Delphi", 238 },
            { "Delphi, el Coleccionista de Memorias", 239 },
            { "Delphi, el Rey de los Dioses", 240 },
            { "Delphi Arceo", 241 },
            { "Delphito, el Bebito", 242 },
            { "Delphito, el Bonito", 243 },
            { "Delphi, el Infinito", 244 },
            { "Jurgentzu, Rey Hades", 245 },
            { "Delphium, el Creador del Tiempo", 246 },
        };

        return characterToShadowMap.TryGetValue(characterName, out int shadowID) ? shadowID : -1;
    }
}
