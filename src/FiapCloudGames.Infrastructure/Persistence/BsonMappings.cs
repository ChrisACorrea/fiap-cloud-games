using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace FiapCloudGames.Infrastructure.Persistence;

public static class BsonMappings
{
    public static void Configure()
    {
        if (BsonClassMap.IsClassMapRegistered(typeof(Usuario)))
        {
            return;
        }

        BsonClassMap.RegisterClassMap<Usuario>(cm =>
        {
            cm.AutoMap();
            cm.MapIdMember(u => u.Id)
                .SetIdGenerator(StringObjectIdGenerator.Instance)
                .SetSerializer(new StringSerializer(BsonType.ObjectId));
            cm.MapMember(u => u.Email).SetSerializer(new EmailSerializer());
            cm.SetIgnoreExtraElements(true);
        });

        BsonClassMap.RegisterClassMap<Jogo>(cm =>
        {
            cm.AutoMap();
            cm.MapIdMember(j => j.Id)
                .SetIdGenerator(StringObjectIdGenerator.Instance)
                .SetSerializer(new StringSerializer(BsonType.ObjectId));
            cm.MapMember(j => j.Preco).SetSerializer(new PrecoSerializer());
            cm.SetIgnoreExtraElements(true);
        });

        BsonClassMap.RegisterClassMap<BibliotecaJogo>(cm =>
        {
            cm.AutoMap();
            cm.MapIdMember(b => b.Id)
                .SetIdGenerator(StringObjectIdGenerator.Instance)
                .SetSerializer(new StringSerializer(BsonType.ObjectId));
            cm.SetIgnoreExtraElements(true);
        });
    }
}

internal sealed class EmailSerializer : IBsonSerializer<Email>
{
    public Type ValueType => typeof(Email);

    public Email Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var endereco = context.Reader.ReadString();
        return new Email(endereco);
    }

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Email value)
    {
        context.Writer.WriteString(value.Endereco);
    }

    object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        => Deserialize(context, args);

    void IBsonSerializer.Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        => Serialize(context, args, (Email)value);
}

internal sealed class PrecoSerializer : IBsonSerializer<Preco>
{
    public Type ValueType => typeof(Preco);

    public Preco Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        context.Reader.ReadStartDocument();
        var valor = context.Reader.ReadDecimal128().ToString();
        var moeda = context.Reader.ReadString();
        context.Reader.ReadEndDocument();
        return new Preco(decimal.Parse(valor), moeda);
    }

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Preco value)
    {
        context.Writer.WriteStartDocument();
        context.Writer.WriteName("valor");
        context.Writer.WriteDecimal128(new Decimal128(value.Valor));
        context.Writer.WriteName("moeda");
        context.Writer.WriteString(value.Moeda);
        context.Writer.WriteEndDocument();
    }

    object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        => Deserialize(context, args);

    void IBsonSerializer.Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        => Serialize(context, args, (Preco)value);
}
