<template>
	<div class="param">
		<div v-if="!showList">
			<div class="form-group">
				<label for="id">Id:</label>
				<input readonly=true class="form-control" id="id" type="text" size="30" v-model="model.Id" />
			</div>
			<div class="form-group">
				<label for="Login">Login:</label>
				<input :readonly="!editing" class="form-control" id="Login" type="text" size="30" v-model="model.Login" />
			</div>
			<div class="form-group">
				<label for="IsAdmin">Admin:</label>
				<input  :disabled="!editing"  class="form-control"
				type="checkbox" id="IsAdmin" v-model="model.IsAdmin"/>
				<!--<input class="form-control" id="Login" type="text" size="30" v-model="model.IsAdmin" />-->
			</div>
			<div class="form-group">
				<label for="Password">Password:</label>
				<input  :readonly="!editing"  class="form-control" id="Password" type="password" size="30" v-model="model.Password" />
			</div>
			<div class="form-group">
				<label for="PasswordNew">Repeat Password:</label>
				<input  :readonly="!editing"  class="form-control" id="PasswordNew" type="password" size="30" v-model="model.PasswordNew" />
			</div>
			<div  v-if="editing" class="form-group">
				<input class="form-control" type="button" value="Save" @click="doSave()">
			</div>
			<div v-if="editing" class="form-group">
				<input class="form-control" type="button" value="Change Password" @click="doSavePassword()">
			</div>
			<div  v-if="editing" class="form-group">
				<input class="form-control" type="button" value="Cancel" @click="doCancel()">
			</div>
			<div  v-if="!editing" class="form-group">
				<input class="form-control" type="button" value="Back" @click="doCancel()">
			</div>
		</div>
		<div v-if="showList">

			<div class="form-group">
				<input class="form-control" type="button" value="Add" @click="doAdd()">
			</div>
			<div class="form-group">
				<table>
					<thead>
						<th>
							<td>Login</td>
							<td>Is Admin</td>
							<td>&nbsp;</td>
							<td>&nbsp;</td>
							<td>&nbsp;</th>
						</tr>
					</thead>
					<tbody>
						<tr v-for="user in users">
							<td>{{user.Login}}</td>
							<td>{{user.IsAdmin}}</td>
							<td>
							<input class="form-control" type="button" value="Delete" @click="doDelete(user.Id)"></td>
							<td>
							<input class="form-control" type="button" value="Update" @click="getById(user.Id,true)"></td>
							<td>
							<input class="form-control" type="button" value="Show" @click="getById(user.Id,false)"></td>
						</tr>
					</tbody>
				</table>
			</div>
		</div>
	</div>
</template>

<script>

	import {EventBus} from '@/services/busService.js'
	import userController from '@/components/user/userController'


	export default {
		name: 'Param',
		mounted(){
			this.getAll();
		},
		data () {
			return {
				showList:true,
				editing:false,
				model:{
					Login :'',
					Password:'',
					PasswordNew:'',
					Id:'',
					IsAdmin:false
				},
				users:[]
			}
		},
		methods:{
			getById : function(id,edit){
				var context = this;
				userController.getById(id,
				function(result){
					context.editing=edit;
					context.showList=false;
					context.model.Id=result.Id;
					context.model.Login=result.Login;
				});
			},
			doAdd : function(){
				var context = this;
				context.editing=true;
				context.model.Id='';
				context.model.Login='';
				context.showList=false;
			},
			doSave : function(){
				var context = this;
				userController.save(context.model,
				function(result){
					context.editing=fase;
					context.model.Id=result;
				});
			},
			doSavePassword:function(){
				userController.savePassword(this.model,
				function(result){
					context.editing=fase;
				});
			},
			doCancel:function(){
				this.editing=false;
				this.showList=true;
				this.getAll();
			},
			doDelete : function(id){
				var context = this;
				userController.delete(id,
				function(result){
					context.getAll();
				});
			},
			getAll : function(){
				var context = this;
				userController.getAll(
				function(data){
					while(context.users.length){
						context.users.pop();
					}
					for(var i=0;i<data.length;i++){
						context.users.push(data[i]);
					}
				});
			}
		}
	}
</script>
