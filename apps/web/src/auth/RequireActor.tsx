import { Navigate, Outlet } from 'react-router-dom';
import { loadActor, type ActorType } from '../auth/actor';

type RequireActorProps = {
  actor: ActorType;
};

export function RequireActor({ actor }: RequireActorProps) {
  const sessionActor = loadActor();
  if (sessionActor !== actor) {
    return <Navigate to="/login" replace />;
  }
  return <Outlet />;
}

export function RequireAnyActor() {
  const sessionActor = loadActor();
  if (!sessionActor) {
    return <Navigate to="/login" replace />;
  }
  return <Outlet />;
}
