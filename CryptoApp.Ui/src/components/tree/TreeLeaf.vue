<template>
	<div>
		<div v-if="canCopyPassword==5"  class="row" style="width:100%;">
			<img :src="`${imgSrc}`" style="width:100%"/>
			<div class="form-group">
				<input class="form-control" type="button" value="Cancel" @click="doCancelMultiStepOperation()">
			</div>
		</div>
		<div v-if="canCopyPassword==4"  class="row" style="width:100%;">
			<div class="form">
				<div class="form-group">
					<label for="file">File Upload:</label>
		        	<input type="file" id="file" ref="file" v-on:change="handleFileUpload()">
				</div>
				<div class="form-group">
					<input class="form-control" type="button" value="Upload" @click="uploadFile()">
				</div>
				<div class="form-group">
					<input class="form-control" type="button" value="Cancel" @click="doCancelMultiStepOperation()">
				</div>
			</div>
		</div>
		<div v-if="canCopyPassword==1"  class="row" style="width:100%;">
			<div class="form">
				<div class="form-group">
					<input class="form-control" type="button" value="Copy Password on Clipboard" @click="copyPasswordToClipboard()">
				</div>
				<div class="form-group">
					<input class="form-control" type="button" value="Cancel" @click="doCancelMultiStepOperation()">
				</div>
			</div>
		</div>
		<div v-if="canCopyPassword==2"  class="row" style="width:100%;">
			<div class="form">
				<div class="form-group">
					<label for="pass">New:</label>
					<input class="form-control" id="pass" type="password" size="30" 
						v-model="model.newPassword" />
				</div>
				<div class="form-group">
					<label for="passr">Repeat:</label>
					<input class="form-control" id="passr" type="password" size="30"
						 v-model="model.newPasswordRepeat" />
				</div>
				<div class="form-group">
					<input class="form-control" type="button" value="Save New Password" @click="savePassword()">
				</div>
				<div class="form-group">
					<input class="form-control" type="button" value="Cancel" @click="doCancelMultiStepOperation()">
				</div>
			</div>
		</div>
		<div v-if="canCopyPassword==0"  class="row" style="width:100%;">
			<div class="form">
				<div v-if="!isEditable" class="form-group">
					<input class="form-control" type="button" value="Load Password" @click="loadPassword()">
				</div>
				<div class="form-group">
					<label for="id">Id:</label>
					<input :readonly="true" class="form-control" readonly='true' id="id" type="text" 
					size="30" v-model="model.id" />
				</div>
				<div class="form-group">
					<label for="label">Label:</label>
					<input :readonly="!isEditable" class="form-control" id="label" type="text" size="30" v-model="model.label" />
				</div>
				<div class="form-group">
					<label for="web">Web:</label>
					<input :readonly="!isEditable" class="form-control" id="web" type="text" size="30" v-model="model.url" />
				</div>
				<div class="form-group">
					<label for="userName">Login:</label>
					<input :readonly="!isEditable" class="form-control" id="username" type="text" size="30" v-model="model.userName" />
				</div>
				 <div class="form-group">
					<label for="notes">Notes:</label>
					<textarea  :readonly="!isEditable" class="form-control" rows="5" id="notes" v-model="model.notes"></textarea>
				</div> 
				<div v-if="!isEditable" class="form-group">
					<a v-if="!isImage" id="attachment" v-bind:href="'/api/attach/'+ model.id">
						<label for="attachment">Attachment</label>
						{{model.hasAttachment}}
					</a>
					<input v-if="isImage" class="form-control" type="button" value="Show Image" @click="showImage()">
					
					
				</div>	
				<div v-if="!isEditable" class="form-group">
					
				</div>	
				<div v-if="!isEditable" class="form-group">
					<input class="form-control" type="button" value="Upload File" @click="uploadAttachment()">
				</div>	
				<div v-if="!isEditable" class="form-group">
					<input class="form-control" type="button" value="Edit" @click="doEdit()">
				</div>	
				<div v-if="!isEditable" class="form-group">
					<input class="form-control" type="button" value="Change Password" @click="doChagePassword()">
				</div>	
				<div v-if="isEditable" class="form-group">
					<input class="form-control" type="button" value="Save" @click="doSave()">
				</div>
				<div v-if="isEditable" class="form-group">
					<input class="form-control" type="button" value="Cancel" @click="doCancel()">
				</div>
				<div v-if="!isEditable" class="form-group">
					<input class="form-control" type="button" value="Move To" @click="doMove()">
				</div>
				<div v-if="!isEditable" class="form-group">
					<input class="form-control" type="button" value="Delete" @click="doDelete()">
				</div>
			</div>
		</div>
	</div>
</template>
<script>
	import treeController from '@/components/tree/treeController'
	import {EventBus} from '@/services/busService.js'
	import clipboardService from '@/services/clipboardService'
	import router from '@/router/index'
	
	
	
	export default {
		name:'tree-leaf',
		data() {
			return  {
				imgSrc:'',
				model:{
					id: '',
					label:'',
					url:'',
					notes:'',
					userName:'',
					parentId:null,
					password:'',
					newPassword:'',
					newPasswordRepeat:'',
					hasAttachment:''
				},
				file:'',
				isEditable:false,
				canCopyPassword:0
			}
		},
		watch: {
			parentData: function () {
				this.loadNode(this.parentData);
			},
			action: function () {
				
			}
		},
		computed: {
			// a computed getter
			isImage: function () {
				if(this.model.hasAttachment){
					var n = this.model.hasAttachment.toUpperCase();
					return n.endsWith(".JPG")||
						n.endsWith(".PNG")||
						n.endsWith(".GIF")||
						n.endsWith(".SWG")||
						n.endsWith(".JPEG");
				}
				return false;
			}
		},
		props: {
			parentData: {
				type: String,
				default () {
					return '';
				}
			},
			action: {
				type: String,
				default () {
					return 'show';
				}
			}
		},
		mounted(){
			this.loadNode(this.parentData);
		},
		methods:{
			doDelete:function(){
				var context = this;
				if(true==confirm("Delete node?")){
					treeController.deleteItem(this.model.id,function(res){
						EventBus.$emit('treeEvent',{
							event:'doLoad',
							id:context.parentData,
							isFolder:context.action=="new"
						});
					});
				}
			},
			doMove:function(){
				EventBus.$emit('treeEvent',{
					event:'doMove',
					id:this.parentData
				});
			},
			doEdit:function(){
				this.isEditable=true;
				EventBus.$emit('treeEvent',{
					id:this.model.id,
					event:'startEditing'
				});
			},
			doCancel:function(){
				var context = this;
				EventBus.$emit('treeEvent',{
					event:'doLoad',
					id:context.parentData,
					isFolder:context.action=="new"
				});
			},
			doSave:function(){
				var context = this;
				if(this.action=="new"){
					this.model.parentId=this.parentData;
				}
				this.model.isFolder=false;
				var bo ={
					Id:this.model.id,
					Title:this.model.label,
					Url:this.model.url,
					UserName:this.model.userName,
					Notes:this.model.notes,
					IsFolder:this.model.isFolder,
					ParentId:this.action=="new"?this.model.parentId:null
				};
				treeController.saveItem(bo,function(result){
					console.log("treeController.saveItem "+context.parentData);
					context.isEditable=false;	
					context.action="show";			
					context.canCopyPassword=0;	
					EventBus.$emit('treeEvent',{
							event:'doLoad',
							id:context.parentData,
							isFolder:context.action=="new"
						});
				});
			},
			loadNode:function(id){
				var context = this;
				treeController.loadNodeFullDataById(this.action,id,function(result){
					context.model.id = result.Id;
					context.model.label = result.Title;
					context.model.url = result.Url;
					context.model.notes = result.Notes;
					context.model.userName = result.UserName;
					context.model.hasAttachment = result.HasAttachment;
					context.canCopyPassword=0;
					if(context.action!="show"){
						context.isEditable=true;					
					}
				
				});
			},
			loadPassword:function(){
				var context = this;
				treeController.loadPassword(this.model.id,function(result){
					context.model.password = result;
					context.canCopyPassword=1;
				});
			},
			copyPasswordToClipboard:function(){
				clipboardService.copyTextToClipboard(this.model.password);
				this.canCopyPassword=0;
				this.model.password = "";
			},
			doCancelMultiStepOperation:function(){
				this.canCopyPassword=0;
				this.model.password = "";
				this.model.newPassword = "";
				this.model.newPasswordRepeat = "";
				this.file='';
				this.imgSrc='';
			},
			doChagePassword:function(){
				this.canCopyPassword=2;
			},
			savePassword:function(){
				var context = this;
				treeController.updatePassword(
					this.model.id,this.model.newPassword,this.model.newPasswordRepeat,
					function(result){
						context.model.password = "";
						context.model.newPassword = "";
						context.model.newPasswordRepeat = "";		
						context.canCopyPassword=0;
					}
				)
				
			},
			downloadAttachment:function(){
				
			},
			uploadAttachment:function(){
				this.canCopyPassword=4;
			},
			handleFileUpload(){
				this.file = this.$refs.file.files[0];
			},
			uploadFile(){
				var context = this;
				treeController.uploadAttachment(
					context.file,
					context.model.id,function(data){
						context.model.hasAttachment=context.file.name;
						context.doCancelMultiStepOperation();
					},function(err){});
			},
			showImage:function(){
				this.imgSrc='/api/attach/'+ this.model.id;
				this.canCopyPassword=5;
			}
		}
	}
</script>