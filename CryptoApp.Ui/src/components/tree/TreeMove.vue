<template>
	<div  class="row" style="width:100%;">
		<div v-if="!canMove" class="form">
			<div  class="form-group">
				<label>Choose target item</label>
			</div>
			<div class="form-group">
				<input class="form-control" type="button" value="Cancel" @click="doCancel()">
			</div>
		</div>
		<div v-if="canMove" class="form">
			<div  class="form-group">
				<label for="id">Moving {{toMove}}/{{toMoveLabel}} to:</label>
				<input class="form-control" readonly='true' id="id" type="text" size="30" v-model="model.id" />
			</div>
			<div  class="form-group">
				<label for="label">Label:</label>
				<input :readonly="isShow" class="form-control" id="label" type="text" size="30" v-model="model.label" />
			</div>
			<div v-if="model.isFolder" class="form-group">
				<input class="form-control" type="button" value="Save" @click="doSave()">
			</div>
			<div class="form-group">
				<input class="form-control" type="button" value="Cancel" @click="doCancel()">
			</div>
		</div>
	</div>
</template>
<script>
	import treeController from '@/components/tree/treeController'
	import {EventBus} from '@/services/busService.js'
	import router from '@/router/index'
	
	export default {
		name:'tree-move',
		data() {
			return  {
				model:{
					id: '',
					label:'',
					isFolder:false
				},
				toMoveLabel:'',
				toMoveId:'',
				isShow:true
			}
		},
		computed:{
			canMove:function(){
				if(this.toMoveId==this.model.id || !this.model.isFolder){
					return false;
				}else{
					return true;
				}
			}
		},
		watch: {
			parentData: function () {
				this.loadNode(this.parentData);
			},
			toMove: function () {
				this.loadLabel(this.toMove);
			}
		},
		props: {
			parentData: {
				type: String,
				default () {
					return '';
				}
			},
			toMove: {
				type: String,
				default () {
					return '';
				}
			}
		},
		mounted(){
			this.loadNode(this.parentData);
			this.loadLabel(this.toMove);
			
		},
		methods:{
			doSave:function(){
				var context = this;
				treeController.moveItem(context.toMoveId,context.model.id,
					function(result){
						EventBus.$emit('treeEvent',{
							id:context.model.id,
							event:'doMoveToRoot'
						});
					});
			},
			doCancel:function(){
				EventBus.$emit('treeEvent',{
					event:'doCancelMove',
					id:this.parentData
				});
			},
			loadNode:function(id){
				var context = this;
				treeController.loadNodeById(id,'show',
					function(result){
						context.model.id = result.Id;
						context.model.label = result.Title;
						context.model.isFolder = result.IsFolder;
					});
			},
			loadLabel:function(id){
				var context = this;
				treeController.loadNodeById(id,'show',
					function(result){
						context.toMoveLabel = result.Title;
						context.toMoveId = result.Id;
					});
			}
		}
	}
</script>