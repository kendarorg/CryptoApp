<template>
	<div class="param">
		<div class="form-group">
			<label for="login">Login:</label>
			<input class="form-control" id="login" type="text" size="30" v-model="model.login" />
		</div>
		<div class="form-group">
			<label for="pwd">Password:</label>
			<input class="form-control" id="id" type="password" size="30" v-model="model.password" />
		</div>
		<div class="form-group">
			<input class="form-control" type="button" value="Login" @click="doLogin()">
		</div>
	</div>
</template>
	
<script>
	
	import {EventBus} from '@/services/busService.js'
	import loginController from '@/components/login/loginController'
	
	
	export default {
		name: 'Param',
		data () {
			return {
				model:{
					login :'',
					password:''
				}
			}
		}, 
		methods:{
			doLogin : function(){
				var context = this;
				var loginModel = {
					Login:context.model.login,
					Password:context.model.password
				};
				loginController.login(loginModel,
					function(result){
						EventBus.$emit('authEvent',{
							event:'loginDone'
						});
						context.$router.push({ name: 'file'});
					});
			},
			doLogogg : function(){
				var context = this;
				var loginModel = this.toModel(this.model);
				loginController.logoff(
					function(result){
						context.$router.push({ name: 'loogin'});
					});
			}
		}
	}
</script>