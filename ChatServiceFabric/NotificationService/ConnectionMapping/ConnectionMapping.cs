
namespace NotificationService.BusinessRules.Hub
{
    public class ConnectionMapping
    {
        private readonly Dictionary<string, HashSet<string>> _connections = new();

        public List<string> UserList
        {
            get
            {
                var _UserList = _connections.Keys.ToList();
                return _UserList;
            }
        }

        public void Add(string key, string connectionId)
        {
            lock (_connections)
            {
                if (!_connections.TryGetValue(key, out HashSet<string>? connections))
                {
                    connections = new HashSet<string>();
                    _connections.Add(key, connections);
                }

                lock (connections)
                {
                    connections.Add(connectionId);
                }
            }
        }

        public HashSet<string> GetConnections(string key)
        {
            if (_connections.TryGetValue(key, out HashSet<string>? connections))
            {
                return connections;
            }

            return new HashSet<string>();
        }

        public Dictionary<string,HashSet<string>> GetConnectionArray()
        {

           
            return _connections;

        }

        public void Remove(string key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string>? connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    return;
                }

                lock (connections)
                {
                    connections.Remove(connectionId);

                    if (connections.Count == 0)
                    {
                        _connections.Remove(key);
                    }
                }
            }
        }
    }

}
