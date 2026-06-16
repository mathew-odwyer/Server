# Kubernetes Development Setup

Follow these steps to prepare the development environment and apply the Kubernetes manifests.

## Development setup

1. Create or update the Caddy secret:
   ```bash
   kubectl create secret generic caddy-secret \
     --from-literal=APP_DOMAIN=internal \
     --from-literal=TLS_EMAIL=internal \
     --from-literal=APP_HOST=gateway \
     --from-literal=APP_PORT=8080 \
     --dry-run=client -o yaml | kubectl apply -f -
   ```

2. Create or update the database secret:
   ```bash
   kubectl create secret generic database-secret \
     --from-literal=SA_PASSWORD='<<CHANGE_PASSWORD>>' \
     --from-literal=ACCEPT_EULA=Y \
     --dry-run=client -o yaml | kubectl apply -f -
   ```

3. Apply the development overlay:
   ```bash
   kubectl apply -k k8s/overlays/development
   ```

4. Verify the deployment status:
   ```bash
   kubectl get pods
   ```

5. Check service and load balancer status:
   ```bash
   kubectl get svc
   ```

If pods are not ready, inspect logs and describe the failing pod:
```bash
kubectl describe pod <pod-name>
kubectl logs <pod-name>
```
