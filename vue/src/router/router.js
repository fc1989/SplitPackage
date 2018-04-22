import Main from '@/views/Main.vue';
import util from '@/libs/util.js';

//title properties are localization keys.

export const loginRouter = {
    path: '/login',
    name: 'login',
    meta: {
        title: 'LogIn'
    },
    component: () => import('@/views/login.vue')
};

export const page404 = {
    path: '/*',
    name: 'error-404',
    meta: {
        title: '404 - Page does not exist'
    },
    component: () => import('@/views/error-page/404.vue')
};

export const page403 = {
    path: '/403',
    meta: {
        title: '403 - You are not authorized'
    },
    name: 'error-403',
    component: () => import('@//views/error-page/403.vue')
};

export const page500 = {
    path: '/500',
    meta: {
        title: '500 - Server error'
    },
    name: 'error-500',
    component: () => import('@/views/error-page/500.vue')
};
export const locking = {
    path: '/locking',
    name: 'locking',
    component: () => import('@/views/main-components/lockscreen/components/locking-page.vue')
};

// A route which is not displayed in the left menu
export const otherRouter = {
    path: '/',
    name: 'otherRouter',
    redirect: '/home',
    component: Main,
    children: [
        { path: 'home', title: 'Menu.Pages.HomePage', name: 'home_index', component: () => import('@/views/home/home.vue') }
    ]
};

// Left menu items
export const appRouter = [
    {
        path: '/admin',
        icon: 'settings',
        title: 'Menu.Modules.Administration',
        name: 'administration',
        component: Main,
        children: [
            { 
                path: 'tenants', 
                title: 'Menu.Pages.Tenants', 
                name: 'tenants', 
                permission: 'Pages.Admin.Tenants', 
                component: () => import('@/views/admin/tenants/tenants.vue') 
            },
            { 
                path: 'productclasses', 
                title: 'Menu.Pages.ProductClasses', 
                name: 'productclasses', 
                permission: 'Pages.Tenant.ProductClasses', 
                component: () => import('@/views/business/productclasses/productclasses.vue') 
            }
        ]
    },
    {
        path: '/business',
        icon: 'briefcase',
        title: 'Menu.Modules.Business',
        name: 'business',
        component: Main,
        children: [
            { 
                path: 'logistics', 
                title: 'Menu.Pages.Logistics', 
                name: 'logistics', 
                permission: 'Pages.Tenant.Logistics', 
                component: () => import('@/views/business/logistics/logistics.vue') 
            },
            { 
                path: 'logisticlines', 
                title: 'Menu.Pages.LogisticLines', 
                name: 'logisticlines', 
                permission: 'Pages.Tenant.LogisticLines', 
                component: () => import('@/views/business/logisticlines/logisticlines.vue') 
            },
            { 
                path: 'products', 
                title: 'Menu.Pages.Products', 
                name: 'products', 
                permission: 'Pages.Tenant.Products', 
                component: () => import('@/views/business/products/products.vue') 
            },
            { 
                path: 'splitrules', 
                title: 'Menu.Pages.SplitRules', 
                name: 'splitrules', 
                permission: 'Pages.Tenant.SplitRules', 
                component: () => import('@/views/business/splitrules/splitrules.vue') 
            },
            { 
                path: 'numfreights', 
                title: 'Menu.Pages.NumFreights', 
                name: 'numfreights', 
                permission: 'Pages.Tenant.NumFreights', 
                component: () => import('@/views/business/numfreights/numfreights.vue') 
            },
            { 
                path: 'weightfreights', 
                title: 'Menu.Pages.WeightFreights', 
                name: 'weightfreights', 
                permission: 'Pages.Tenant.WeightFreights', 
                component: () => import('@/views/business/weightfreights/weightfreights.vue') 
            }
        ]
    },
    {
        path: '/system',
        icon: 'gear-b',
        title: 'Menu.Modules.System',
        name: 'system',
        component: Main,
        children: [
            { 
                path: 'users', 
                title: 'Menu.Pages.Users', 
                name: 'users', 
                permission: 'Pages.Tenant.Users', 
                component: () => import('@/views/admin/users/users.vue') 
            },
            { 
                path: 'roles', 
                title: 'Menu.Pages.Roles', 
                name: 'roles', 
                permission: 'Pages.Tenant.Roles', 
                component: () => import('@/views/admin/roles/roles.vue') 
            }
        ]
    }
];

// All the routes defined above should be written in the routers below
export const routers = [
    loginRouter,
    otherRouter,
    locking,
    ...appRouter,
    page500,
    page403,
    page404
];
