using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

class TestStruct : NetworkBehaviour
{
    struct MyStruct : INetworkSerializable
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public string SpriteId;

        // INetworkSerializable
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Position);
            serializer.SerializeValue(ref Rotation);
            serializer.SerializeValue(ref SpriteId);
        }
    }

    // Method to load sprite by id
    Sprite LoadSprite(string spriteId)
    {
        // Load Texture2D from Resources
        Texture2D texture = Resources.Load<Texture2D>(spriteId);
        if (texture != null)
        {
            return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
        return null;
    }

    [Rpc(SendTo.NotServer)]
    void MyServerRpc(MyStruct myStruct)
    {
        transform.position = myStruct.Position;

        // Load sprite using SpriteId
        Sprite receivedSprite = LoadSprite(myStruct.SpriteId);
        if (receivedSprite != null)
        {
            // Apply the sprite to a component (example: SpriteRenderer)
            GetComponent<Image>().sprite = receivedSprite;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            MyServerRpc(
                new MyStruct
                {
                    Position = transform.position,
                    Rotation = transform.rotation,
                    SpriteId = "grass" // Send the identifier for the sprite
                }); // Client -> Server
        }
    }
}
