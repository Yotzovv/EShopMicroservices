# PostgreSQL Authentication Issue Resolution

## Problem Description

When running the Basket.API microservice with multiple startup projects in Visual Studio, the application fails to connect to the PostgreSQL database container with authentication errors.

### Initial Error Messages

```
PostgresException (0x80004005): 28P01: password authentication failed for user "postgres"
```

Later evolved to:
```
NpgsqlException (0x80004005): No password has been provided but the backend requires one (in SASL/SCRAM-SHA-256)
```

## Root Cause Analysis

The issue stems from a mismatch between:
1. **PostgreSQL Container Configuration**: How the database is set up for authentication
2. **Application Connection String**: How the .NET application attempts to connect
3. **Docker Volume Persistence**: Old authentication settings persisting across container restarts

## Environment Setup

- **Solution Structure**: `src/Services/Basket/Basket.API/` and `src/Services/Discount/Discount.Grpc/`
- **Docker Compose**: Infrastructure services (PostgreSQL + Redis) run in containers
- **Application Execution**: Multiple startup projects run directly in Visual Studio (not containerized)
- **Connection**: Application connects from host machine to containerized PostgreSQL

## Solutions Attempted

### ❌ **Attempt 1: Remove POSTGRES_HOST_AUTH_METHOD=trust**
**Action Taken:**
```yaml
# Removed from docker-compose.override.yml
environment:
  - POSTGRES_USER=postgres
  - POSTGRES_PASSWORD=postgres
  - POSTGRES_DB=BasketDb
  # - POSTGRES_HOST_AUTH_METHOD=trust  # REMOVED
```

**Result:** Failed - PostgreSQL defaulted to password authentication but connection string configuration wasn't aligned.

### ❌ **Attempt 2: Use MD5 Password Authentication**
**Action Taken:**
```yaml
environment:
  - POSTGRES_USER=postgres
  - POSTGRES_PASSWORD=postgres
  - POSTGRES_DB=BasketDb
  - POSTGRES_HOST_AUTH_METHOD=md5
```

**Result:** Failed - This approach works but requires precise password handling, which conflicts with development simplicity.

### ⚠️ **Attempt 3: Remove Password from Connection String**
**Action Taken:**
```json
{
  "ConnectionStrings": {
    "Database": "Server=localhost;Port=5433;Database=BasketDb;User Id=postgres;Include Error Detail=true"
    // Removed: Password=postgres
  }
}
```

**Result:** Partially successful but requires proper PostgreSQL trust authentication setup.

### 🔄 **Current Solution (In Progress): Trust Authentication with Fresh Volumes**

**Understanding the Issue:**
- PostgreSQL containers with existing volumes retain old `pg_hba.conf` authentication settings
- `POSTGRES_HOST_AUTH_METHOD=trust` only applies during initial database initialization
- Docker volumes persist authentication configuration across container restarts

**Required Steps:**
1. **Stop Containers and Remove Volumes:**
   ```bash
   docker-compose -f docker-compose.yml -f docker-compose.override.yml down -v
   ```

2. **Configure Trust Authentication:**
   ```yaml
   basketdb:
     container_name: basketdb
     environment:
       - POSTGRES_USER=postgres
       - POSTGRES_PASSWORD=postgres
       - POSTGRES_DB=BasketDb
       - POSTGRES_HOST_AUTH_METHOD=trust
   ```

3. **Update Connection String (No Password):**
   ```json
   "Database": "Server=localhost;Port=5433;Database=BasketDb;User Id=postgres;Include Error Detail=true"
   ```

4. **Start Fresh Containers:**
   ```bash
   docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d catalogdb basketdb distributedCache
   ```

## Technical Details

### PostgreSQL Authentication Methods
- **trust**: No password required (development only)
- **md5**: Password required, MD5 hashed
- **scram-sha-256**: Password required, stronger encryption

### pg_hba.conf Configuration
The PostgreSQL `pg_hba.conf` file controls client authentication:
```
# TYPE  DATABASE        USER            ADDRESS                 METHOD
host    all             all             127.0.0.1/32            trust
host    all             all             ::1/128                 trust
host    all             all             all                     trust  # For external connections
```

### Connection Context
- **Application Location**: Host machine (Visual Studio)
- **Database Location**: Docker container
- **Connection Type**: External TCP/IP connection (not localhost socket)
- **Required Rule**: `host all all all trust`

## Best Practices for Development

1. **Use Trust Authentication for Development**: Simplifies local development setup
2. **Always Remove Volumes When Changing Auth**: `docker-compose down -v`
3. **Keep Connection Strings Simple**: Avoid passwords when using trust authentication
4. **Document Infrastructure Dependencies**: Clear setup instructions for team members

## Production Considerations

⚠️ **Security Warning**: Trust authentication should **NEVER** be used in production environments. For production:
- Use `scram-sha-256` or `md5` authentication
- Implement proper password management
- Use environment variables for sensitive configuration
- Consider certificate-based authentication

## Files Modified

1. **`docker-compose.override.yml`**: PostgreSQL authentication configuration
2. **`src/Services/Basket/Basket.API/appsettings.json`**: Connection string without password
3. **Docker volumes**: Removed and recreated for fresh authentication setup

## Current Status

🔄 **In Progress**: Implementing fresh container creation with trust authentication to resolve the SCRAM-SHA-256 authentication requirement error.

## Next Steps

1. Complete the fresh container setup with trust authentication
2. Verify connection from Basket.API to PostgreSQL
3. Test full application functionality with multiple startup projects
4. Document the final working configuration for team reference