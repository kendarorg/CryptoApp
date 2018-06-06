import Vue from 'vue'
import VueRouter from 'vue-router'
//import the hello component
import Login from '@/components/login/Login'
import Errors from '@/components/errors/Errors'
import FileLoader from '@/components/fileLoader/FileLoader'
import Tree from '@/components/tree/Tree'
import List from '@/components/list/List'
import Users from '@/components/user/User'

//tell vue to use the router
Vue.use(VueRouter)
//define your routes
const routes = [
	{ path: '/', component: Login,name:'login' },
	{ path: '/errors', component: Errors,name:'errors' },
	{ path: '/file', component: FileLoader,name:'file'},
	{ path: '/tree', component: Tree,name:'tree'},
	{ path: '/list', component: List,name:'list'},
	{ path: '/users', component: Users,name:'users'}
	//{ path: '/param/:id', component: Paramdetails,name:'param_id'}
];

// Create the router instance and pass the `routes` option
// You can pass in additional options here, but let's
// keep it simple for now.
export default new VueRouter({
	routes, // short for routes: routes
	mode: 'history'
});
