package com.khadamati.api.rbac;

import java.util.Arrays;
import java.util.stream.Collectors;

import org.aspectj.lang.annotation.Aspect;
import org.aspectj.lang.annotation.Before;
import org.springframework.security.access.AccessDeniedException;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.GrantedAuthority;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.stereotype.Component;

@Aspect
@Component
public class RoleAuthorizationAspect {

    @Before("@within(rolesAllowed) && execution(* *(..))")
    public void checkClassRoles(RolesAllowed rolesAllowed) {
        enforce(rolesAllowed);
    }

    @Before("@annotation(rolesAllowed)")
    public void checkMethodRoles(RolesAllowed rolesAllowed) {
        enforce(rolesAllowed);
    }

    private void enforce(RolesAllowed rolesAllowed) {
        Authentication authentication = SecurityContextHolder.getContext().getAuthentication();
        if (authentication == null || !authentication.isAuthenticated()) {
            throw new AccessDeniedException("Authentication required");
        }

        var allowed = Arrays.stream(rolesAllowed.value())
                .map(role -> "ROLE_" + role.name())
                .collect(Collectors.toSet());

        boolean hasRole = authentication.getAuthorities().stream()
                .map(GrantedAuthority::getAuthority)
                .anyMatch(allowed::contains);

        if (!hasRole) {
            throw new AccessDeniedException("Insufficient role");
        }
    }
}
