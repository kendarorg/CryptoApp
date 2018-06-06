<template>
	<div id="app">
		<div class="container" style="width:100%;">
			<div class="row">
				<div v-if="!isLoggedIn" class="col col-lg-3">
					<router-link v-bind:to="'/'">Login</router-link>
				</div>
				<div v-if="isLoggedIn" class="col col-lg-3">
					<a href="#" @click="logoff()">Logoff</a>
				</div>
				<div v-if="isLoggedIn" class="col col-lg-3">
					<router-link v-bind:to="'/file'">Files</router-link>
				</div>
				<div v-if="isLoggedIn" class="col col-lg-3">
					<router-link v-bind:to="'/users'">Users</router-link>
				</div>
				<div v-if="hasTree" class="col col-lg-3">
					<router-link v-bind:to="'/tree'">Tree</router-link>
				</div>
				<div v-if="hasTree" class="col col-lg-3">
					<router-link v-bind:to="'/list'">List</router-link>
				</div>
				<div class="col col-lg-3">
					<router-link v-bind:to="'/about'">About</router-link>
				</div>
				<!--<div class="col col-lg-4">
					<router-link v-bind:to="'/param'">Param Link</router-link>
				</div>-->
			</div>
			<br>
			<div class="row">
				<!-- the router outlet, where all matched components would ber viewed -->
				<router-view></router-view>
			</div>
		</div>
	</div>
</template>

<script>
	import {EventBus} from '@/services/busService.js'
	import authService from '@/services/authService.js'
	import router from '@/router/index'
	
	export default {
		name: 'app',
		data:{
			isLoggedIn:false,
			hasTree:false
		},
		mounted(){
			// Listen to the event.
			EventBus.$on('authEvent', this.authEventHandler);
		},
		beforeDestroy(){
			// Stop listening.
			EventBus.$off('authEvent', this.authEventHandler);
		},
		methods:{
			logoff:function(){
				var context = this;
				authService.logoff().
					then(function(data){
						context.isLoggedIn = false;
						context.hasTree = false;
						router.push({ name: 'login'});
					});
			},
			authEventHandler:function(evt){
				var context = this;
				if(evt.event){
					console.log("TREE EVT"+evt.event);
					if(evt.event=='loginDone'){
						context.isLoggedIn = true;
					}else  if(evt.event=='fileLoaded'){
						context.hasTree = true;
					}
				}
			}
		}
	}
</script>
<!-- styling for the component -->
<style>	
	#app {
		font-family: 'Avenir', Helvetica, Arial, sans-serif;
		-webkit-font-smoothing: antialiased;
		-moz-osx-font-smoothing: grayscale;
		color: #2c3e50;
		margin-top: 10px;
		margin-left: 10px;
	}
</style>