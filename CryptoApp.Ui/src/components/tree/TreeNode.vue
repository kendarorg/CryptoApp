<template>
	<div  class="row" style="width:100%;">
		<div class="form">
			<div  class="form-group">
				<label for="id">Id:</label>
				<input class="form-control" readonly='true' id="id" type="text" size="30" v-model="model.id" />
			</div>
			<div  class="form-group">
				<label for="label">Label:</label>
				<input :readonly="!isEditable" class="form-control" id="label" type="text" size="30" v-model="model.label" />
			</div>
			<div v-if="!isEditable" class="form-group">
				<input class="form-control" type="button" value="Edit" @click="doEdit()">
			</div>
			<div v-if="isEditable" class="form-group">
				<input class="form-control" type="button" value="Save" @click="doSave()">
			</div>
			<div v-if="isEditable" class="form-group">
				<input class="form-control" type="button" value="Cancel" @click="doCancel()">
			</div>
			<div v-if="!isEditable" class="form-group">
				<input class="form-control" type="button" value="Add Node" @click="doAddNode()">
			</div>
			<div v-if="!isEditable" class="form-group">
				<input class="form-control" type="button" value="Add Leaf" @click="doAddLeaf()">
			</div>
			<div v-if="!isEditable" class="form-group">
				<input class="form-control" type="button" value="Move To" @click="doMove()">
			</div>
			<div v-if="!isEditable" class="form-group">
				<input class="form-control" type="button" value="Delete" @click="doDelete()">
			</div>
		</div>
	</div>
</template>
<script>
	import treeController from '@/components/tree/treeController'
	import {EventBus} from '@/services/busService.js'
	import router from '@/router/index'
	
	export default {
		name:'tree-node',
		data() {
			return  {
				model:{
					id: '',
					label:'',
					parentId:null
				},
				isEditable:false
			}
		},
		watch: {
			parentData: function () {
				this.loadNode(this.parentData);
			},
			action: function () {
				
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
			doAddNode:function(){
				EventBus.$emit('treeEvent',{
					event:'doAddNode',
					id:this.model.id,
					parentId:this.model.id
				});
			},
			doAddLeaf:function(){
				EventBus.$emit('treeEvent',{
					event:'doAddLeaf',
					id:this.model.id,
					parentId:this.model.id
				});
			},
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
			doSave:function(){
				var context = this;
				if(this.action=="new"){
					this.model.parentId=this.parentData;
				}
				this.model.isFolder=true;
				
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
					context.isEditable=false;	
					context.action="show";				
					EventBus.$emit('treeEvent',{
							event:'doLoad',
							id:context.parentData,
							isFolder:true
						});
				});
			},
			doEdit:function(){
				this.isEditable=true;
				EventBus.$emit('treeEvent',{
					event:'startEditing'
				});
			},
			doCancel:function(){
				//if(this.action=="new"){
				var context = this;
				EventBus.$emit('treeEvent',{
					event:'doLoad',
					id:context.parentData,
					isFolder:true
				});
			},
			loadNode:function(id){
				var context = this;
				
				
				treeController.loadNodeById(id,this.action,function(result){
					context.model.id = result.Id;
					context.model.label = result.Title;
					if(context.action!="show"){
						context.isEditable=true;					
					}
				});
			}
		}
	}
</script>