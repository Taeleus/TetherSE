using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRageMath;
using DamageBatch = VRage.Collections.ListReader<VRage.MyTuple<ulong, long, int, VRage.Game.Entity.MyEntity, VRage.Game.Entity.MyEntity, VRage.Collections.ListReader<VRage.MyTuple<VRageMath.Vector3D, object, float>>>>;

internal class CoreGun : MyCubeBlock, IMyTerminalBlock
{
    public CoreGun() { BlockDefinition.Id = new MyDefinitionId("CoreGun"); }
}
internal class CoreHand : MyEntity, IMyGunObject<MyDeviceBase>
{
    public bool IsShooting => false;
    public MyDeviceBase GunBase => null;
}
internal sealed class FakeWeaponCore
{
    internal class Part
    {
        public bool Shooting;
        public int Shots;
        public List<Action<int, bool>> Observers = new List<Action<int, bool>>();
    }
    private readonly Dictionary<MyEntity, Dictionary<int, Part>> weapons = new Dictionary<MyEntity, Dictionary<int, Part>>();
    private Action<DamageBatch> damage;
    public bool ThrowPolling;
    public bool ThrowCleanup;
    public readonly HashSet<MyEntity> SupportEntities = new HashSet<MyEntity>();
    public int ObserverCount => weapons.Values.Sum(parts => parts.Values.Sum(p => p.Observers.Count));
    public bool DamageRegistered => damage != null;
    public void Add(MyEntity entity, params int[] ids) { weapons[entity] = ids.ToDictionary(id => id, id => new Part()); }
    public Part State(MyEntity entity, int id) { return weapons[entity][id]; }
    public void Event(MyEntity entity, int id, int kind, bool active)
    {
        foreach (var callback in weapons[entity][id].Observers.ToArray()) callback(kind, active);
    }
    public void RespondOnRequest() { MyAPIGateway.Utilities.OnApiRequest = Respond; }
    public void Respond()
    {
        var methods = new Dictionary<string, Delegate>
        {
            ["GetCoreWeapons"] = new Action<ICollection<MyDefinitionId>>(defs => defs.Add(new MyDefinitionId("CoreGun"))),
            ["HasCoreWeaponBase"] = new Func<MyEntity, bool>(entity => weapons.ContainsKey(entity)),
            ["GetBlockWeaponMap"] = new Func<IMyTerminalBlock, IDictionary<string, int>, bool>((entity, map) =>
            {
                if (SupportEntities.Contains((MyEntity)entity) || !weapons.TryGetValue((MyEntity)entity, out var parts)) return false;
                foreach (var id in parts.Keys) map.Add("Part" + id, id);
                return true;
            }),
            ["IsWeaponShootingBase"] = new Func<MyEntity, int, bool>((entity, id) =>
            {
                if (ThrowPolling) throw new InvalidOperationException("API temporarily unavailable");
                return State(entity, id).Shooting;
            }),
            ["GetShotsFiredBase"] = new Func<MyEntity, int, int>((entity, id) => State(entity, id).Shots),
            ["RegisterEventMonitor"] = new Action<MyEntity, int, Action<int, bool>>((entity, id, callback) => State(entity, id).Observers.Add(callback)),
            ["UnRegisterEventMonitor"] = new Action<MyEntity, int, Action<int, bool>>((entity, id, callback) =>
            {
                if (ThrowCleanup) throw new InvalidOperationException("WC already unloaded");
                State(entity, id).Observers.Remove(callback);
            }),
            ["DamageHandler"] = new Action<long, int, Action<DamageBatch>>((id, kind, callback) =>
            {
                if (ThrowCleanup) throw new InvalidOperationException("WC already unloaded");
                damage = kind == 0 ? null : callback;
            })
        };
        MyAPIGateway.Utilities.Broadcast(methods);
    }
    public void Hit(object target, float amount)
    {
        var hits = new List<MyTuple<Vector3D, object, float>> { new MyTuple<Vector3D, object, float>(default, target, amount) };
        var batch = new List<MyTuple<ulong, long, int, MyEntity, MyEntity, ListReader<MyTuple<Vector3D, object, float>>>>
        {
            new MyTuple<ulong, long, int, MyEntity, MyEntity, ListReader<MyTuple<Vector3D, object, float>>>(1, 1, 0, null, null, new ListReader<MyTuple<Vector3D, object, float>>(hits))
        };
        damage?.Invoke(new DamageBatch(batch));
    }
}
