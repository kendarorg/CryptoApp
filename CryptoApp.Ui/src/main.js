// The Vue build version to load with the `import` command

//import the vue instance
import Vue from 'vue'
//import Vuec from 'vue-container'
//Vue.use(Vuec);
//import the App component
import App from './App'
//import all the routing data
import router from '@/router/index'

import BootstrapVue from 'bootstrap-vue'
Vue.use(BootstrapVue);


import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap-vue/dist/bootstrap-vue.css'
import 'font-awesome/css/font-awesome.min.css'
import '@/assets/main.css'


import TreeMenu from '@/components/TreeMenu'
import TreeLeaf from '@/components/tree/TreeLeaf'
import TreeNode from '@/components/tree/TreeNode'
import TreeNone from '@/components/tree/TreeNone'
import TreeMove from '@/components/tree/TreeMove'

Vue.component('tree-menu', TreeMenu);
Vue.component('tree-move', TreeMove);
Vue.component('tree-none', TreeNone);
Vue.component('tree-node', TreeNode);
Vue.component('tree-leaf', TreeLeaf);


import {EventBus} from '@/services/busService.js'

//instatinat the vue instance
new Vue({
	//define the selector for the root component
	el: '#app',
	//pass the template to the root component
	template: '<App/>',
	//declare components that the root component can access
	components: { App },
	//pass in the router to the Vue instance
	router
}).$mount('#app')//mount the router on the app